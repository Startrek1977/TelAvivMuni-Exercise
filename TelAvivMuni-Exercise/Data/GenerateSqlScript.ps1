$json = Get-Content 'D:\WORK\TelAvivMuni-Exercise\TelAvivMuni-Exercise\TelAvivMuni-Exercise\Data\Products.json' | ConvertFrom-Json

$sb = [System.Text.StringBuilder]::new()

# Header
[void]$sb.AppendLine('-- =============================================')
[void]$sb.AppendLine('-- TelAviv Municipality Exercise')
[void]$sb.AppendLine('-- Database Creation Script with Full Seed Data')
[void]$sb.AppendLine('-- Generated: ' + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'))
[void]$sb.AppendLine('-- Total Products: ' + $json.Count)
[void]$sb.AppendLine('-- =============================================')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('-- Create database (optional - uncomment if needed)')
[void]$sb.AppendLine('-- CREATE DATABASE TelAvivMuniExercise;')
[void]$sb.AppendLine('-- GO')
[void]$sb.AppendLine('-- USE TelAvivMuniExercise;')
[void]$sb.AppendLine('-- GO')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('-- =============================================')
[void]$sb.AppendLine('-- Create Product Table')
[void]$sb.AppendLine('-- =============================================')
[void]$sb.AppendLine('IF OBJECT_ID(''dbo.Product'', ''U'') IS NOT NULL')
[void]$sb.AppendLine('    DROP TABLE dbo.Product;')
[void]$sb.AppendLine('GO')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('CREATE TABLE dbo.Product')
[void]$sb.AppendLine('(')
[void]$sb.AppendLine('    Id       INT            NOT NULL PRIMARY KEY,')
[void]$sb.AppendLine('    Code     NVARCHAR(20)   NULL,')
[void]$sb.AppendLine('    Name     NVARCHAR(100)  NULL,')
[void]$sb.AppendLine('    Category NVARCHAR(50)   NULL,')
[void]$sb.AppendLine('    Price    DECIMAL(10, 2) NOT NULL DEFAULT 0,')
[void]$sb.AppendLine('    Stock    INT            NOT NULL DEFAULT 0')
[void]$sb.AppendLine(');')
[void]$sb.AppendLine('GO')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('-- Create indexes for common query patterns')
[void]$sb.AppendLine('CREATE INDEX IX_Product_Code ON dbo.Product (Code);')
[void]$sb.AppendLine('CREATE INDEX IX_Product_Category ON dbo.Product (Category);')
[void]$sb.AppendLine('CREATE INDEX IX_Product_Name ON dbo.Product (Name);')
[void]$sb.AppendLine('GO')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('-- =============================================')
[void]$sb.AppendLine('-- Seed Data (' + $json.Count + ' products)')
[void]$sb.AppendLine('-- =============================================')
[void]$sb.AppendLine('SET NOCOUNT ON;')
[void]$sb.AppendLine('GO')
[void]$sb.AppendLine('')

# Generate INSERT statements in batches of 1000
$batchSize = 1000
for ($i = 0; $i -lt $json.Count; $i += $batchSize) {
    $end = [Math]::Min($i + $batchSize, $json.Count)
    [void]$sb.AppendLine('-- Batch ' + ([Math]::Floor($i / $batchSize) + 1) + ': Products ' + ($i + 1) + ' - ' + $end)
    [void]$sb.AppendLine('INSERT INTO dbo.Product (Id, Code, Name, Category, Price, Stock)')
    [void]$sb.AppendLine('VALUES')

    for ($j = $i; $j -lt $end; $j++) {
        $p = $json[$j]
        $code = if ($p.Code) { "N'" + $p.Code.Replace("'", "''") + "'" } else { 'NULL' }
        $name = if ($p.Name) { "N'" + $p.Name.Replace("'", "''") + "'" } else { 'NULL' }
        $category = if ($p.Category) { "N'" + $p.Category.Replace("'", "''") + "'" } else { 'NULL' }
        $comma = if ($j -lt $end - 1) { ',' } else { ';' }
        [void]$sb.AppendLine('    (' + $p.Id + ', ' + $code + ', ' + $name + ', ' + $category + ', ' + $p.Price + ', ' + $p.Stock + ')' + $comma)
    }
    [void]$sb.AppendLine('GO')
    [void]$sb.AppendLine('')
}

[void]$sb.AppendLine('SET NOCOUNT OFF;')
[void]$sb.AppendLine('GO')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('PRINT ''Product table created and seeded with ' + $json.Count + ' products successfully.'';')
[void]$sb.AppendLine('GO')

# Write to file
$sb.ToString() | Out-File -FilePath 'D:\WORK\TelAvviMinu-Exercise\TelAvviMinu-Exercise\TelAvivMuni-Exercise\Data\CreateProductsDatabase.sql' -Encoding UTF8

Write-Host 'SQL script generated successfully!'
Write-Host ('File: D:\WORK\TelAvviMinu-Exercise\TelAvviMinu-Exercise\TelAvivMuni-Exercise\Data\CreateProductsDatabase.sql')
Write-Host ('Total products: ' + $json.Count)
