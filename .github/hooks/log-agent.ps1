param(
    [string] $LogPath = "C:\Downloads NVMe\ASP_Projekt_Sljeme\.github\hooks\agent_log.txt"
)

# Procitaj payload iz pipelinea ili STDIN-a
$payload = if ($input) {
    $input | Out-String
} else {
    [Console]::In.ReadToEnd()
}

# Ako je payload prazan, upisi fallback poruku
if ([string]::IsNullOrWhiteSpace($payload)) {
    $payload = "[HOOK TRIGGERED] " + (Get-Date -Format "yyyy-MM-dd HH:mm:ss") + " | input empty"
} else {
    $payload = $payload.TrimEnd("`r", "`n")
}

# Upisi u log
$payload | Out-File -FilePath $LogPath -Append -Encoding utf8