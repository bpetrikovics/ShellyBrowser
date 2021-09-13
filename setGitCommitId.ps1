# Get build running directory

$scriptPath = split-path -parent $MyInvocation.MyCommand.Path
$gitPath = 'c:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\Git\cmd\git'

try {
    $v = Invoke-Expression "& '$gitPath' rev-parse --verify --short HEAD"
}
catch [System.Management.Automation.CommandNotFoundException] {
    Write-Output "FATAL: git not found"
    exit
}

# Letters are incompatible with AssemblyVersion.cs so we remove them
$v = $v -replace "v", ""
# Version format is major[.minor[.build[.revision]] so we remove them
$v = $v -replace "-(\D.*)", ''
$v = $v -replace "-", '.'

Write-Output $v

# We replace versions inside AssemblyInfo.cs content
$info = (Get-Content ($scriptPath + "/ShellyBrowser.App/Properties/AssemblyInfo.cs"))
$av = '[assembly: AssemblyInformationalVersion("'+$v+'")]'
$info = $info -replace '\[assembly: AssemblyInformationalVersion\("(.*)"\)]', $av
Set-Content -Path ($scriptPath + "/ShellyBrowser.App/Properties/AssemblyInfo.cs") -Value $info -Encoding UTF8
