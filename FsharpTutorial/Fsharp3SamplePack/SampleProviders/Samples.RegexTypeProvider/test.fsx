#r @".\bin\Debug\Samples.RegexTypeProvider.dll"

open Samples.FSharp.RegexTypeProvider

type T = RegexTyped< @"(?<AreaCode>^\d{3})-(?<PhoneNumber>\d{3}-\d{4}$)">
let reg = T() 
let result = T.IsMatch("425-123-2345")
let r = reg.Match("425-123-2345").AreaCode.Value //r equals "425"
