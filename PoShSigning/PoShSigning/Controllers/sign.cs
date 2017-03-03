using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Management.Automation; // Copy([PSObject].Assembly.Location) C:\PoShSigning
using System.Collections;
using System.Collections.ObjectModel;

namespace PoShSigning.Controllers
{
    [RoutePrefix("api/sign")]
    public class sign : ApiController
    {
        // POST api/sign
        public string Post([FromBody]string value)
        {
            var temp = Path.GetTempFileName();
            using (StreamWriter sw = new StreamWriter(temp))
            {
                sw.Write(value);
                sw.Flush();
                sw.Close();
            }

            List<string> output = new List<string>();
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddCommand(
                    @"$cert = @(Get-ChildItem Cert:\LocalMachine\My -CodeSigningCert)[0]; " +
                    @"[void](Set-AuthentiCodeSignature -Certificate $cert -FilePath " + temp + "); " +
                    @"return get-content -path " + temp + ";");

                Collection<PSObject> response = PowerShellInstance.Invoke();
                foreach (var psObject in response)
                {
                    output.Add(psObject.ToString());
                }
            }


            return String.Join("\r\n", output.ToArray());
        }
    }
}
