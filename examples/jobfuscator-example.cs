/******************************************************************************
 * JObfuscator WebApi interface usage example.
 *
 * In this example we will obfuscate sample source with custom options.
 *
 * Version        : v1.1.0
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
// should the source code be compressed (both input & compressed)
//
myJObfuscator.EnableCompression = true;

//
// extract int / char / double / string literals into encrypted array tables (early pipeline)
//
myJObfuscator.ArrayIntCrypt = true;
myJObfuscator.ArrayCharCrypt = true;
myJObfuscator.ArrayDoubleCrypt = true;
myJObfuscator.ArrayStringCrypt = true;

//
// global obfuscation options
//
// when disabled will discard any @Obfuscate annotation declaration
// in the Java source code
//
// you can disable a particular obfuscation strategy globally if it
// fails or you don't want to use it without modifying the source codes
//
// by default all obfuscation strategies are enabled
//

//
// change linear code execution flow to non-linear version
//
myJObfuscator.MixCodeFlow = true;

//
// rename variable names to random string values
//
myJObfuscator.RenameVariables = true;

//
// rename method names to random string values
//
myJObfuscator.RenameMethods = true;

//
// shuffle order of methods in the output source
//
myJObfuscator.ShuffleMethods = true;

//
// encrypt integers using more than 15 floating point math functions from the java.lang.Math.* class
//
myJObfuscator.IntsMathCrypt = true;

//
// encrypt doubles using java.lang.Math.* style transforms
//
myJObfuscator.DblsMathCrypt = true;

//
// encrypt strings using polymorphic encryption algorithms
//
myJObfuscator.CryptStrings = true;

//
// extract individual character literals used in encryption into auxiliary tables
//
myJObfuscator.StringCharVault = true;

//
// derive occasional integer literals from double / Math-heavy expressions
//
myJObfuscator.IntsFromDoubleMath = true;

//
// insert opaque predicates and mixer chains in the control flow
//
myJObfuscator.OpaqueMixerChain = true;

//
// replace straightforward boolean checks with heavier equivalent expressions
//
myJObfuscator.ComplexifyBooleans = true;

//
// wrap blocks with benign try/finally noise
//
myJObfuscator.TryFinallyNoise = true;

//
// for each method, extract all possible integers from the code and store them in an array
//
myJObfuscator.IntsToArrays = true;

//
// for each method, extract all possible doubles from the code and store them in an array
//
myJObfuscator.DblsToArrays = true;

//
// source code in Java format
//
const string SourceCode =
    """
    import java.util.*;
    import java.lang.*;
    import java.io.*;

    //
    // you must include custom annotation
    // to enable entire class or a single
    // method obfuscation
    //
    @Obfuscate
    class Ideone
    {
        //@Obfuscate
        public static double calculateSD(double numArray[])
        {
            double sum = 0.0, standardDeviation = 0.0;
            int length = numArray.length;

            for(double num : numArray) {
                sum += num;
            }

            double mean = sum/length;

            for(double num: numArray) {
                standardDeviation += Math.pow(num - mean, 2);
            }

            return Math.sqrt(standardDeviation/length);
        }

        //
        // selective obfuscation strategies
        // can be applied for the entire
        // class or a single method (by
        // default all obfuscation strategies
        // are enabled when you use @Obfuscate
        // annotation alone)
        //
        //@Obfuscate(
        //  ints_math_crypt = true,
        //  dbls_math_crypt = true,
        //  crypt_strings = true,
        //  string_char_vault = true,
        //  rename_methods = false,
        //  rename_variables = true,
        //  shuffle_methods = true,
        //  mix_code_flow = true,
        //  ints_from_double_math = true,
        //  opaque_mixer_chain = true,
        //  complexify_booleans = true,
        //  try_finally_noise = true,
        //  ints_to_arrays = true,
        //  dbls_to_arrays = true
        // )
        public static void main(String[] args) {

            double[] numArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double SD = calculateSD(numArray);

            System.out.format("Standard Deviation = %.6f", SD);
        }
    }
    """;

//
// by default all options are enabled, both helper random numbers
// generation & obfuscation strategies, so we can just simply call:
//
var result = await myJObfuscator.ObfuscateJavaSourceAsync(SourceCode);

//
// it's also possible to pass a Java source file path instead of a string e.g.
//
// var result = await myJObfuscator.ObfuscateJavaFileAsync("/path/to/project/source.java");

//
// result object holds the obfuscation results as well as other information
//
// result?.Error            - error code (see JObfuscator.Error*)
// result?.Output           - obfuscated code
// result?.Demo             - was it used in demo mode (invalid or empty activation key was used)
// result?.CreditsLeft      - usage credits left after this operation
// result?.CreditsTotal    - total number of credits for this activation code
// result?.Expired         - if this was the last usage credit for the activation key it will be set to true
//
if (result is not null)
{
    //
    // display obfuscated code
    //
    if (result.Error == JObfuscator.ErrorSuccess && result.Output is not null)
        Console.WriteLine(result.Output);
    else
        throw new InvalidOperationException("An error occurred, error code: " + result.Error);
}
else
{
    throw new InvalidOperationException("Something unexpected happen while trying to obfuscate the code.");
}
