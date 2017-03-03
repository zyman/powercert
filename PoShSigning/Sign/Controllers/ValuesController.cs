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
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public string Post([FromBody]string script)
        {
            var temp = Path.GetTempFileName() + ".ps1";
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

                PowerShellInstance.AddCommand(@"C:\PoShSigning\Sign-ThroughAPI.ps1");
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

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
