param(
    [ValidateSet('Status', 'Enable', 'Remove')] [string]$Action = 'Status',
    [ValidateRange(1, 65535)] [int]$Port = 5080,
    [Parameter(Mandatory)] [string]$ProgramPath
)

# Feste, appbezogene Regel; keine globale Freigabe, kein Ändern von Netzwerkprofilen.
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)
$ruleName = "AP2-Lernwerk-Heimnetz-TCP-$Port"
$resolvedProgram = [IO.Path]::GetFullPath($ProgramPath)
if (-not [IO.File]::Exists($resolvedProgram) -or [IO.Path]::GetExtension($resolvedProgram) -ine '.exe') { throw 'Der Programmpfad ist ungültig.' }

function Get-TrainerStatus {
    $networks = @(Get-NetIPConfiguration | Where-Object { $_.NetProfile.NetworkCategory -eq 'Private' -and $_.IPv4DefaultGateway } | ForEach-Object {
        foreach ($address in $_.IPv4Address) {
            if ($address.IPAddress -match '^(10\.|192\.168\.|172\.(1[6-9]|2[0-9]|3[01])\.)') {
                @{ Address = $address.IPAddress; Name = $_.InterfaceAlias }
            }
        }
    })
    $rule = Get-NetFirewallRule -Name $ruleName -ErrorAction SilentlyContinue
    $ready = $false
    if ($rule) {
        $portFilter = $rule | Get-NetFirewallPortFilter
        $addressFilter = $rule | Get-NetFirewallAddressFilter
        $applicationFilter = $rule | Get-NetFirewallApplicationFilter
        $ready = ($rule.Enabled -eq 'True' -and $rule.Direction -eq 'Inbound' -and $rule.Action -eq 'Allow' -and [string]$rule.Profile -eq 'Private' -and
            [string]$portFilter.Protocol -eq 'TCP' -and [string]$portFilter.LocalPort -eq [string]$Port -and
            @($addressFilter.RemoteAddress).Count -eq 1 -and [string]$addressFilter.RemoteAddress -eq 'LocalSubnet' -and
            [string]$applicationFilter.Program -ieq $resolvedProgram -and [string]$rule.EdgeTraversalPolicy -eq 'Block')
    }
    @{ PrivateNetworks = $networks; RuleReady = [bool]$ready; RuleExists = [bool]$rule }
}

try {
    if ($Action -eq 'Status') {
        Get-TrainerStatus | ConvertTo-Json -Depth 5 -Compress
        exit 0
    }
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    if (-not ([Security.Principal.WindowsPrincipal]$identity).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { exit 5 }
    if ($Action -eq 'Enable') {
        if (@((Get-TrainerStatus).PrivateNetworks).Count -eq 0) { exit 6 }
        # Nur die eigene benannte Regel ersetzen, niemals fremde Firewall-Regeln bearbeiten.
        Get-NetFirewallRule -Name $ruleName -ErrorAction SilentlyContinue | Remove-NetFirewallRule
        New-NetFirewallRule -Name $ruleName -DisplayName "AP2 Lernwerk – Heimnetz (TCP $Port)" -Description 'Private Heimnetz-Freigabe des AP2 Lernwerks.' -Direction Inbound -Action Allow -Enabled True -Profile Private -Protocol TCP -LocalPort $Port -Program $resolvedProgram -RemoteAddress LocalSubnet -EdgeTraversalPolicy Block | Out-Null
    }
    else { Get-NetFirewallRule -Name $ruleName -ErrorAction SilentlyContinue | Remove-NetFirewallRule }
    exit 0
}
catch { exit 1 }
