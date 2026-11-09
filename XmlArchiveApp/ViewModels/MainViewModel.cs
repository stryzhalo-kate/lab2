using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using XmlArchiveApp.Models;
using XmlArchiveApp.Services;
using XmlArchiveApp.Strategies;

namespace XmlArchiveApp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ArchiveAnalyzer _analyzer = new();
    private readonly HtmlTransformer _transformer = new();
    private string _keyword = "";
    private string _filePath = "";
    private string _selectedMethod = "DOM";
    private string _status = "Оберіть XML-файл";
    private SearchCriteria _criteria = new();
    private Dictionary<string, List<string>> _availableValues = new();
    private WebViewSource? _htmlSource;

    private string? _selectedAuthor, _selectedMaterialType,
                    _selectedFaculty, _selectedDepartment, _selectedCreatedDate;

    public MainViewModel()
    {
        LoadFileCommand = new Command(async () => await LoadFileAsync());
        SearchCommand = new Command(async () => await SearchAsync());
        TransformCommand = new Command(async () => await TransformAsync());
        ClearCommand = new Command(Clear);
        ExitCommand = new Command(async () => await ExitAsync());
    }

    public string FilePath { get => _filePath; set => Set(ref _filePath, value); }
    public string Status { get => _status; set => Set(ref _status, value); }
    public SearchCriteria Criteria { get => _criteria; set => Set(ref _criteria, value); }
    public Dictionary<string, List<string>> AvailableValues
    {
        get => _availableValues;
        set => Set(ref _availableValues, value);
    }
    public WebViewSource? HtmlSource { get => _htmlSource; set => Set(ref _htmlSource, value); }

    public List<string> Methods { get; } = new() { "SAX", "DOM", "LINQ to XML" };
    public string SelectedMethod { get => _selectedMethod; set => Set(ref _selectedMethod, value); }

    
    public ObservableCollection<string> AuthorValues { get; } = new();
    public ObservableCollection<string> MaterialTypeValues { get; } = new();
    public ObservableCollection<string> FacultyValues { get; } = new();
    public ObservableCollection<string> DepartmentValues { get; } = new();
    public ObservableCollection<string> CreatedDateValues { get; } = new();

    public string? SelectedAuthor { get => _selectedAuthor; set => Set(ref _selectedAuthor, value); }
    public string? SelectedMaterialType { get => _selectedMaterialType; set => Set(ref _selectedMaterialType, value); }
    public string? SelectedFaculty { get => _selectedFaculty; set => Set(ref _selectedFaculty, value); }
    public string? SelectedDepartment { get => _selectedDepartment; set => Set(ref _selectedDepartment, value); }
    public string? SelectedCreatedDate { get => _selectedCreatedDate; set => Set(ref _selectedCreatedDate, value); }
    public string Keyword { get => _keyword; set => Set(ref _keyword, value); }
    public ObservableCollection<ArchiveItem> Results { get; } = new();

    
    public ICommand LoadFileCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand TransformCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ExitCommand { get; }

    
    public async Task LoadFileAsync()
    {
        try
        {
            var xmlType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xml" } }
            });
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть XML-файл",
                FileTypes = xmlType
            });
            if (file == null) return;

            FilePath = file.FullPath;
            ApplyStrategy();

            AvailableValues = _analyzer.GetAttributeValues(FilePath);
            Fill(AuthorValues, "Author");
            Fill(MaterialTypeValues, "MaterialType");
            Fill(FacultyValues, "Faculty");
            Fill(DepartmentValues, "Department");
            Fill(CreatedDateValues, "CreatedDate");

            Clear();
            Status = "Файл завантажено: " + Path.GetFileName(FilePath);
        }
        catch (Exception ex)
        {
            await AlertAsync("Помилка", ex.Message);
        }
    }

    public async Task SearchAsync()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            await AlertAsync("Увага", "Спочатку оберіть XML-файл.");
            return;
        }

        Criteria = new SearchCriteria
        {
            Author = SelectedAuthor ?? "",
            MaterialType = SelectedMaterialType ?? "",
            Faculty = SelectedFaculty ?? "",
            Department = SelectedDepartment ?? "",
            CreatedDate = SelectedCreatedDate ?? "",
            Keyword = Keyword
        };

        if (Criteria.IsEmpty)
        {
            await AlertAsync("Увага", "Оберіть параметр або введіть ключове слово.");
            return;
        }

        try
        {
            ApplyStrategy();
            Results.Clear();
            foreach (var item in _analyzer.Search(FilePath, Criteria))
                Results.Add(item);
            Status = $"Метод: {SelectedMethod}. Знайдено: {Results.Count}";
        }
        catch (Exception ex)
        {
            await AlertAsync("Помилка", ex.Message);
        }
    }

    public async Task TransformAsync()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            await AlertAsync("Увага", "Спочатку оберіть XML-файл.");
            return;
        }

        try
        {
            string xslPath = await CopyXslToCacheAsync();
            string html = _transformer.Transform(FilePath, xslPath);

            string outPath = Path.ChangeExtension(FilePath, ".html");
            await File.WriteAllTextAsync(outPath, html);

            HtmlSource = new HtmlWebViewSource { Html = html };
            Status = "HTML збережено: " + outPath;
        }
        catch (Exception ex)
        {
            await AlertAsync("Помилка", ex.Message);
        }
    }

   
    public void Clear()
    {
        Results.Clear();
        HtmlSource = null;
        SelectedAuthor = null;
        SelectedMaterialType = null;
        SelectedFaculty = null;
        SelectedDepartment = null;
        SelectedCreatedDate = null;
        Keyword = "";
        Criteria = new SearchCriteria();
    }

    public async Task ExitAsync()
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page == null) return;

        bool yes = await page.DisplayAlertAsync(
            "Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
        if (yes) Application.Current?.Quit();
    }

    
    private void ApplyStrategy()
    {
        IAnalysisStrategy strategy = SelectedMethod switch
        {
            "SAX" => new SaxStrategy(),
            "LINQ to XML" => new LinqToXmlStrategy(),
            _ => new DomStrategy()
        };
        _analyzer.SetStrategy(strategy);
    }

    private void Fill(ObservableCollection<string> target, string key)
    {
        target.Clear();
        if (AvailableValues.TryGetValue(key, out var values))
            foreach (var v in values) target.Add(v);
    }

    private static async Task<string> CopyXslToCacheAsync()
    {
        string target = Path.Combine(FileSystem.CacheDirectory, "archive.xsl");
        using Stream source = await FileSystem.OpenAppPackageFileAsync("archive.xsl");
        using FileStream dest = File.Create(target);
        await source.CopyToAsync(dest);
        return target;
    }

    private static async Task AlertAsync(string title, string message)
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page != null) await page.DisplayAlertAsync(title, message, "OK");
    }

    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
