# Clear the Add-ons menu if one exists.
$psISE.CurrentPowerShellTab.AddOnsMenu.SubMenus.Clear()

$menuAdded = $psISE.CurrentPowerShellTab.AddOnsMenu.SubMenus.Add(
    "_Sign script",
    {
        $sig = (Invoke-WebRequest -Uri "http://localhost:50933/api/sign" -Method Post -Body $("=" + $psISE.CurrentFile.Editor.Text) -UseDefaultCredentials).Content;
        $psISE.CurrentFile.Editor.Clear();
        $psISE.CurrentFile.Editor.InsertText( $sig.Replace("\r\n", "`r`n")).Replace("\""", """")
        $psISE.CurrentFile.Editor.Text = $psISE.CurrentFile.Editor.Text.Substring(1,$psISE.CurrentFile.Editor.Text.Length-2)
        #$psISE.CurrentFile.Editor.InsertText($sig.Replace("\r\n", "`r`n").Replace("\""", """"))
        
    },
    "Alt+S") 

# $psISE.CurrentFile.Editor.Text = $sig.Replace("\r\n", "`r`n")