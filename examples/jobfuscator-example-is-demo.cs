/******************************************************************************
 * JObfuscator WebApi interface usage example.
 *
 * In this example we will verify our activation key status.
 *
 * Version        : v1.0.0
 * Language       : C#
 * Author         : Bartosz Wójcik
 * Web page       : https://www.pelock.com
 *
 *****************************************************************************/

using PELock;

//
// create JObfuscator class instance (we are using our activation key)
//
var myJObfuscator = new JObfuscator("ABCD-ABCD-ABCD-ABCD");

//
// login to the service
//
var result = await myJObfuscator.LoginAsync();

//
// result object holds the information about the license
//
// result?.Demo           - is it a demo mode (invalid or empty activation key was used)
// result?.CreditsLeft    - usage credits left after this operation
// result?.CreditsTotal    - total number of credits for this activation code
// result?.StringLimit     - Max. source code size allowed (it's 1500 bytes for demo mode)
//
if (result is not null)
{
    Console.WriteLine("Demo version status - " + (result.Demo == true ? "true" : "false"));
    Console.WriteLine("Usage credits left - " + result.CreditsLeft);
    Console.WriteLine("Total usage credits - " + result.CreditsTotal);
    Console.WriteLine("Max. source code size - " + result.StringLimit);
}
else
{
    throw new InvalidOperationException("Something unexpected happen while trying to login to the service.");
}
