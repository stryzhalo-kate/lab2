<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="utf-8" indent="yes"/>

	<xsl:template match="/archive">
		<html>
			<head>
				<meta charset="utf-8"/>
				<title>Електронний архів</title>
				<style>
					body { font-family: Arial, sans-serif; margin: 20px; }
					table { border-collapse: collapse; width: 100%; }
					th, td { border: 1px solid #444; padding: 6px 10px; text-align: left; }
					th { background-color: #e0e0e0; }
				</style>
			</head>
			<body>
				<h1>Електронний архів факультету</h1>
				<table>
					<tr>
						<th>№</th>
						<th>Назва</th>
						<th>Автор</th>
						<th>Факультет</th>
						<th>Кафедра</th>
						<th>Вид</th>
						<th>Обсяг</th>
						<th>Дата створення</th>
					</tr>
					<xsl:apply-templates select="material"/>
				</table>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="material">
		<tr>
			<td>
				<xsl:value-of select="@id"/>
			</td>
			<td>
				<xsl:value-of select="title"/>
			</td>
			<td>
				<xsl:value-of select="author/@name"/>
			</td>
			<td>
				<xsl:value-of select="author/@faculty"/>
			</td>
			<td>
				<xsl:value-of select="author/@department"/>
			</td>
			<td>
				<xsl:value-of select="@type"/>
			</td>
			<td>
				<xsl:value-of select="@size"/>
			</td>
			<td>
				<xsl:value-of select="@created"/>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>