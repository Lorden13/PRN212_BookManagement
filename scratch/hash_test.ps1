$privateKey = '9jS7vSNXDmhVssdcgjRunoyKQpeOSndd3DzK8sBetn2yzPqdYAn9R0+*^R&$S80kC4bUuKveVJVt'
$passwords = @('1', '123', '1234', '12345', '123456', 'admin', 'khoa', 'trankhoa', '123@123a')
foreach ($p in $passwords) {
    $raw = $p + $privateKey
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($raw)
    $sha = [System.Security.Cryptography.SHA256]::Create()
    $hashBytes = $sha.ComputeHash($bytes)
    $hashStr = ($hashBytes | ForEach-Object { $_.ToString('x2') }) -join ''
    Write-Output "$p -> $hashStr"
}
