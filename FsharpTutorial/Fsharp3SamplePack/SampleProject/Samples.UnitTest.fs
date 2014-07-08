// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

[<Support.Helper.SampleAttributes.Sample("UnitTest")>]
module Samples.UnitTest

open System
open System.Collections.Generic
open Microsoft.VisualStudio.TestTools.UnitTesting;
open Support.Helper

[<Support("msTest")>]
let dummy() = ()
// To use MSTest, please make sure to
//  * include Microsoft.VisualStudio.QualityTools.UnitTestFramework reference 
//  * open Microsoft.VisualStudio.TestTools.UnitTesting namespace

[<TestClass>]
type Test() = 

    [<TestMethod>]
    member this.Test1() =
        Assert.AreEqual(1, 1)

    [<TestMethod>]
    member this.Test2() =
        Assert.Greater(2,1)

[<Category("MSTest");
  Title("MSTest");
  Description("Define and execute a unit test. You can use mstest.exe /testcontainer:SampleProject.exe to execute the test case. Another alternative is to use the F# unit test template: http://visualstudiogallery.msdn.microsoft.com/432eb82c-345e-4502-be56-015fe051a210")>]
let msTest() = 
    let T = Test()
    T.Test1()
