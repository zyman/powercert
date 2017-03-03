$body = "trololo"
$re = Invoke-WebRequest -Uri "http://localhost:50933/api/sign" -Method Post -Body "=$body" -UseDefaultCredentials
$re.Content