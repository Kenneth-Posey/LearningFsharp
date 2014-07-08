#r @".\bin\Debug\TypeProviderTemplate1.dll"
#r @"C:\Program Files\Open XML SDK\V2.5\lib\DocumentFormat.OpenXml.dll"

type T = Samples.ShareInfo.TPTest.TPTestType<"AA.docx">
let t = T("MRXYZ.docx")
t.Person <- "Mr. XYZ"
t.ContactInformation <- "ABC@ABC.com"
t.MyCompany <- "ABC Company"
t.MyName <- "John Lee"
t.NewProduct <- "ABC New Product"
t.Close()