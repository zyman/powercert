using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sign.Controllers
{
    //[Authorize]
    public class SignController : ApiController
    {
        

        // POST api/values
        public string Post([FromBody]string script)
        {
            var temp = Path.ChangeExtension(Path.GetTempFileName(), "ps1");
            using (StreamWriter sw = new StreamWriter(temp))
            {
                sw.Write(script);
                sw.Flush();
                sw.Close();
            }

            List<string> output = new List<string>();
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                //PowerShellInstance.AddScript(
                //    @"$cert = @(Get-ChildItem Cert:\LocalMachine\My -CodeSigningCert)[0]; " +
                //    @"[void](Set-AuthentiCodeSignature -Certificate $cert -FilePath " + temp + "); " +
                //    @"return get-content -path " + temp + ";");


                //PowerShellInstance.AddScript(@"C:\PoShSigning\Sign-ThroughAPI.ps1");
                //PowerShellInstance.AddParameter("ScriptPath", temp);

                PowerShellInstance.AddCommand(@"C:\PoShSigning\Source-Code\Sign-ThroughAPI.ps1");
                PowerShellInstance.AddArgument(temp);

                Collection<PSObject> response = PowerShellInstance.Invoke();
                Collection<ErrorRecord> err = PowerShellInstance.Streams.Error.ReadAll();

                foreach (var psObject in response)
                {
                    output.Add(psObject.ToString());
                }
            }


            return String.Join("\r\n", output.ToArray());
        }
    }
}
