// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

[<Support.Helper.SampleAttributes.Sample("LINQ Queries")>]
module Samples.Linq

open System
open System.Collections.Generic
open System.Linq
open System.Data.Linq.SqlClient
open Support.Helper

// Record for Select 
[<Support("LINQ09")>]
let dummy1() =()
type Record = {Upper :string; Lower:string}
[<Support("LINQ10")>]
let dummy2() = ()
type REcord2 ={ Digit:string; Even:bool}

[<Category("LINQ");
  Title("where - Simple");
  Description("This sample uses where to find all elements of an array less than 5.")>]
let LINQ01() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]
 
    let results = 
        query {
            for n in numbers do
            where (n < 5)
            select n;
        } 

    for n in results do 
        printfn "%A" n

    // sample output
    //    4 
    //    1 
    //    3 
    //    2 
    //    0


[<Category("LINQ");
  Title("where - Indexed");
  Description("This sample demonstrates an indexed Where clause that returns digits whose name is shorter than their value.")>]

let LINQ02() =
    let digits = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
    let results = 
        query {
            for d in digits do
            select d
        } 
    results
    |> Seq.mapi (fun i d -> if d.Length < i then Some d else None) 
    |> Seq.choose id
    |> Seq.iter(fun n -> printfn "The word %s is shorter than its value." n)

    //sample output
    //    The word five is shorter than its value. 
    //    The word six is shorter than its value. 
    //    The word seven is shorter than its value. 
    //    The word eight is shorter than its value. 
    //    The word nine is shorter than its value.


[<Category("LINQ");
  Title("distinct - Simple");
  Description("This sample uses Distinct to remove duplicate elements in a sequence.")>]

let LINQ03() =
    let factorsOf300 = [2;2;3;5;5]

    let results = 
        query {
            for n in factorsOf300 do
            distinct
        } 

    for n in results do 
        printfn "%d" n

    // sample output
    //    2
    //    3
    //    5


[<Category("LINQ");
  Title("Union - Simple");
  Description("This sample uses Union to create one sequence that contains the unique values from both arrays.")>]

let LINQ04() =
    let numbersA = [ 0; 2; 4; 5; 6; 8; 9 ]
    let numbersB = [ 1; 3; 5; 7; 8 ]

    let results = numbersA.Union numbersB 
    for n in results do 
        printfn "%d" n

    //sample output
    //    0
    //    2
    //    4
    //    5
    //    6
    //    8
    //    9
    //    1
    //    3
    //    7


[<Category("LINQ");
  Title("Intersect - Simple");
  Description("This sample uses Intersect to create one sequence that contains the common values shared by both arrays.")>]

let LINQ05() =
    let numbersA = [ 0; 2; 4; 5; 6; 8; 9 ]
    let numbersB = [ 1; 3; 5; 7; 8 ]

    let results = numbersA.Intersect numbersB

    for n in results do 
        printfn "%d" n

    // sample output
    //    5
    //    8


[<Category("LINQ");
  Title("Except - Simple");
  Description("This sample uses Except to create a sequence that contains the values from numbersAthat are not also in numbersB.")>]

let LINQ06() =
    let numbersA = [ 0; 2; 4; 5; 6; 8; 9 ]
    let numbersB = [ 1; 3; 5; 7; 8 ]

    let results = numbersA.Except numbersB
    for n in results do 
        printfn "%d" n

    //sample output
    //    0
    //    2
    //    4
    //    6
    //    9

  
[<Category("LINQ");
  Title("select - Simple");
  Description("This sample uses select to produce a sequence of ints one higher than those in an existing array of integers.")>]

let LINQ07() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]
    
    let results = 
        query {
            for n in numbers do
            select (n + 1)
        } 
    for n in results do 
        printfn "%d" n
    
    //sample output
    //    6
    //    5
    //    2
    //    4
    //    10
    //    9
    //    7
    //    8
    //    3
    //    1


[<Category("LINQ");
  Title("select - Transformation");
  Description("This sample uses select to produce a sequence of strings representing the text version of a sequence of integers.")>]

let LINQ08() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]
    let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]

    let results = 
        query {
                for n in numbers do
                select (strings.[n])
        }

    for e in results do 
        printfn "%s" e
    
    //sample output
    //    Number strings:
    //    five
    //    four
    //    one
    //    three
    //    nine
    //    eight
    //    six
    //    seven
    //    two
    //    zero


[<Category("LINQ");
  Title("select - Record Types 1");
  Description("This sample uses select to produce a sequence of the uppercase and lowercase versions of each word in the original array.")>]

let LINQ09() =
    
    let words = ["aPPLE"; "BlUeBeRrY"; "cHeRry" ]
                    
    let results = 
        query {
                for w in words do
                select {Upper = w.ToUpper(); Lower =w.ToLower()}
        } 

    for result in results do
        printfn "Uppercase: %s, Lowercase: %s" result.Upper result.Lower

    //sample output
    //    Uppercase: APPLE, Lowercase: apple
    //    Uppercase: BLUEBERRY, Lowercase: blueberry
    //    Uppercase: CHERRY, Lowercase: cherry

    
[<Category("LINQ");
  Title("select - Record Types 2");
  Description("This sample uses select to produce a sequence containing text representations of digits and whether their length is even or odd.")>]

let LINQ10() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]    
    let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
    
    let results = 
        query {
                for n in numbers do
                select {Digit = strings.[n]; Even = (n % 2) = 0}
        }

    for r in results do 
        printfn "The digit %s is %s." r.Digit (if r.Even then "even" else "odd")

    //sample output
    //    The digit five is odd.
    //    The digit four is even.
    //    The digit one is odd.
    //    The digit three is odd.
    //    The digit nine is odd.
    //    The digit eight is even.
    //    The digit six is even.
    //    The digit seven is odd.
    //    The digit two is even.
    //    The digit zero is even.


[<Category("LINQ");
  Title("select - Filtered");
  Description("This sample combines select and where to make a simple query that returns the text form of each digit less than 5.")>]

let LINQ11() =    
    let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
    let digits = strings
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let results = 
        query {
                for n in numbers do
                where (n < 5)
                select digits.[n]
        } 

    for result in results do 
        printfn "%s" result
    
    //sample output
    //    four
    //    one
    //    three
    //    two
    //    zero

 
[<Category("LINQ");
  Title("select - Compound");
  Description("This sample uses a compound from clause to make a query that returns all pairs of numbers from both arrays such that the number from numbersA is less than the number from numbersB.")>]

let LINQ12() =
    let numbersA = [0; 2; 4; 5; 6; 8; 9]
    let numbersB = [1; 3; 5; 7; 8]

    let results = 
        query {
                for a in numbersA do
                for b in numbersB do
                where (a < b)
                select (a,b)
        } 

    for (n,e) in results do 
        printfn "%d is less than %d" n e
    
    //sample output
    //    0 is less than 1
    //    0 is less than 3
    //    0 is less than 5
    //    0 is less than 7
    //    0 is less than 8
    //    2 is less than 3
    //    2 is less than 5
    //    2 is less than 7
    //    2 is less than 8
    //    4 is less than 5
    //    4 is less than 7
    //    4 is less than 8
    //    5 is less than 7
    //    5 is less than 8
    //    6 is less than 7
    //    6 is less than 8


[<Category("LINQ");
  Title("exists - Simple");
  Description("This sample uses Any to determine if any of the words in the array contain the substring 'ei'.")>]

let LINQ13() =
    let words = ["believe"; "relief"; "receipt"; "field"]
    let iAfterE =
        query {
            for w in words do
            exists (w.Contains("ei"))
        }

    printfn "There is a word that contains in the list that contains 'ei': %A" iAfterE

    //sample output
    //    There is a word that contains in the list that contains 'ei': true


[<Category("LINQ");
  Title("all - simple");
  Description("This sample uses All to determine whether an array contains only odd numbers.")>]

let LINQ14() =
    let numbers = [1;11;3;19;41;65;19]

    let onlyOdd =
        query {
            for n in numbers do
            all (n % 2 = 1)
        }
    printfn "The list contains only odd numbers: %A" onlyOdd

    //sample output
    //    The list contains only odd numbers: true



[<Category("LINQ");
  Title("take - simple");
  Description("This sample uses Take to get only the first 3 elements of the array.")>]

let LINQ15() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let results = 
        query {
            for n in numbers do
            take 3
        } 

    for result in results do 
        printfn "%d" result

    //sample output
    //    5 
    //    4 
    //    1


[<Category("LINQ");
  Title("skip - simple");
  Description("This sample uses Skip to get all but the first 4 elements of the array.")>]

let LINQ16() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let results = 
        query {
            for n in numbers do
            skip 4
        } 

    for result in results do 
        printfn "%A" result
    
    //sample output
    //    9 
    //    8 
    //    6 
    //    7 
    //    2 
    //    0


[<Category("LINQ");
  Title("takeWhile - simple");
  Description("This sample uses TakeWhile to return elements starting from the beginning of the array until a number is hit that is not less than 6.")>]

let LINQ17() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let results = 
        query {
            for n in numbers do
            takeWhile (n < 6)
        } 

    for result in results do 
        printfn "%A" result
    
    //sample output
    //    5
    //    4
    //    1
    //    3


[<Category("LINQ");
  Title("skipWhile - simple");
  Description("This sample uses SkipWhile to get the elements of the array starting from the first element divisible by 3.")>]

let LINQ18() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let results = 
        query {
            for n in numbers do
            skipWhile (n % 3 <> 0)
        } 
    
    for result in results do 
        printfn "%A" result
    
    //sample output
    //    3
    //    9
    //    8
    //    6
    //    7
    //    2
    //    0



[<Category("LINQ");
  Title("sortBy - Simple 1");
  Description("This sample uses orderby to sort a list of words alphabetically.")>]

let LINQ19() =
    let words = ["cherry"; "apple"; "blueberry"]

    let results = 
        query {
            for w in words do
            sortBy w
            select w
        } 
    
    for result in results do 
        printfn "%s" result
    
    //samples output
    //    apple
    //    blueberry
    //    cherry


[<Category("LINQ");
  Title("sortBy - Simple 2");
  Description("This sample uses orderby to sort a list of words by length.")>]

let LINQ20() =
    let words = ["cherry"; "apple"; "blueberry"]

    let results = 
        query {
            for w in words do 
            sortBy (w.Length)
            select w
        } 
        
    for result in results do 
        printfn "%s" result
    
    //sample output
    //    apple
    //    cherry
    //    blueberry


[<Category("LINQ");
  Title("sortByDescending - Simple");
  Description("This sample uses orderby and descending to sort a list of doubles from highest to lowest.")>]

let LINQ21() =
    let doubles = [1.7M; 2.3M; 1.9M; 4.1M; 2.9M]

    let results = 
        query {
            for d in doubles do
            sortByDescending d
            select d
        } 
    for result in results do 
        printfn "%M" result
    
    //sample output
    //    4.1
    //    2.9
    //    2.3
    //    1.9
    //    1.7

[<Category("LINQ");
  Title("thenBy - Simple");
  Description("This sample uses a compound orderby to sort a list of digits, first by length of their name, and then alphabetically by the name itself.")>]

let LINQ22() =
    let digits = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]

    let results = 
        query {
            for d in digits do
            sortBy d.Length
            thenBy d
            select d
        } 

    for result in results do 
        printfn "%s" result
    
    //sample output
    //    one
    //    six
    //    two
    //    five
    //    four
    //    nine
    //    zero
    //    eight
    //    seven
    //    three


[<Category("LINQ");
  Title("groupValBy - Simple 1");
  Description("This sample uses group by to partition a list of numbers by their remainder when divided by 5.")>]

let LINQ23() =
    let digits = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]

    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let resultGroups = 
        query {       
            for n in numbers do
            groupValBy n (n % 5) into g
            select (g.Key, g.ToArray())
        } 
    
    for (n,resultGroup) in resultGroups do 
        printfn "Numbers with a remainder of %A when divided by 5:" n
        for x in resultGroup do 
            printfn"%d" x
    

    //sample output
    //    Numbers with a remainder of 0 when divided by 5:
    //    5
    //    0
    //    Numbers with a remainder of 4 when divided by 5:
    //    4
    //    9
    //    Numbers with a remainder of 1 when divided by 5:
    //    1
    //    6
    //    Numbers with a remainder of 3 when divided by 5:
    //    3
    //    8
    //    Numbers with a remainder of 2 when divided by 5:
    //    7
    //    2


[<Category("LINQ");
  Title("groupValBy - Simple 2");
  Description("This sample uses group by to partition a list of words by their first letter.")>]

let LINQ24() =
    let words = ["blueberry"; "chimpanzee"; "abacus"; "banana"; "apple"; "cheese" ]

    let resultGroups = 
        query {
            for w in words do
            groupValBy w w.[0] into g
            select (g.Key, g.ToArray())
        } 

    for (n,resultGroup) in resultGroups do
        printfn "Words that start with the letter '%A'" n
        for x in resultGroup do 
            printfn"%s" x
    
    //sample output
    //    Words that start with the letter 'b':
    //    blueberry
    //    banana
    //    Words that start with the letter 'c':
    //    chimpanzee
    //    cheese
    //    Words that start with the letter 'a':
    //    abacus
    //    apple



[<Category("LINQ");
  Title("head - Condition");
  Description("This sample uses First to find the first element in the array that starts with 'o'.")>]

let LINQ25() =
    let strings = [ "zero"; "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]

    let startsWithO =
        query {
            for s in strings do
            where (s.[0] = 'o')
            head
        }

    printfn "A string starting with 'o': %s" startsWithO

    //sample output
    //    A string starting with 'o': one

[<Category("LINQ");
  Title("headOrDefault - Simple");
  Description("This sample uses FirstOrDefault to try to return the first element of the sequence, unless there are no elements, in which case the default value for that type is returned.")>]

let LINQ26() =

    let numbers : int list = []

    let firstNumOrDefault =
        query {
            for n in numbers do
            headOrDefault
        }

    printfn "%A" firstNumOrDefault

    //sample output
    //    0

[<Category("LINQ");
  Title("nth");
  Description("This sample uses ElementAt to retrieve the second number greater than 5 from an array.")>]
let LINQ27() =
    let numbers2 = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let fourthLowNum = 
        query {
            for n in numbers2 do
            where (n > 5)
            nth 1
        }

    printfn "Second number > 5: %A" fourthLowNum

    //sample output
    //    Second number > 5: 8


[<Category("LINQ");
  Title("count - Simple");
  Description("This sample uses Count to get the number of unique factors of 300.")>]
let LINQ28() =
    let factorsOf300 = [2;2;3;5;5]

    let uniqueFactors = 
        query {
            for n in factorsOf300 do
            distinct
            count
        } 

    printfn "There are %A unique factors of 300." uniqueFactors

    //sample output
    //    There are 3 unique factors of 300.


[<Category("LINQ");
  Title("count - Conditional");
  Description("This sample uses Count to get the number of odd ints in the array.")>]
let LINQ29() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let oddNumbers  = numbers.Count(fun n -> n % 2 = 1)

    printfn "There are %A odd numbers in the list." oddNumbers

    //sample output
    //  There are 5 odd numbers in the list.


[<Category("LINQ");
  Title("sumBy - Simple");
  Description("This sample uses Sum to get the total of the numbers in an array.")>]
let LINQ30() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]

    let numSum = 
        query {
            for n in numbers do
            sumBy n
        }
    printfn "The sum of the numbers is %A." numSum

    //sample output
    //  The sum of the numbers is 45.

[<Category("LINQ");
  Title("sumBy - Projection");
  Description("This sample uses Sum to get the total number of characters of all words in the array.")>]
let LINQ31() =
    let words = ["cherry"; "apple"; "blueberry"]

    let totalChars = 
        query {
            for w in words do
            sumBy (w.Length)
        }
    printfn "There are a total of %A characters in these words." totalChars

    //sample output
    //  There are a total of 20 characters in these words.
    

[<Category("LINQ");
  Title("minBy - simple");
  Description("This sample uses Min to get the lowest number in an array.")>]
let LINQ32() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]
    let minNum = 
        query { 
            for n in numbers do
            minBy n 
        }
    printfn "The minimum number is %A." minNum
    
    //sample output
    //    The minimum number is 0.



[<Category("LINQ");
  Title("minBy - Projection");
  Description("This sample uses Min to get the length of the shortest word in an array.")>]
let LINQ33() =
    let words = ["cherry"; "apple"; "blueberry"]
    let shortestWord = 
        query {
             for w in words do
             minBy w.Length
         }
    printfn "The shortest word is %A characters long." shortestWord


    //sample output
    //  The shortest word is 5 characters long.


[<Category("LINQ");
  Title("maxBy - Simple");
  Description("This sample uses Max to get the highest number in an array.")>]
let LINQ34() =
    let numbers = [ 5; 4; 1; 3; 9; 8; 6; 7; 2; 0 ]
    let maxNum = 
        query { 
            for n in numbers do 
            maxBy n 
        }
    printfn "The maximum number is %A." maxNum

    //sample output
    //  The maximum number is 9.

[<Category("LINQ");
  Title("maxBy - Projection");
  Description("This sample uses Max to get the length of the longest word in an array.")>]
let LINQ35() =
    let words = ["cherry"; "apple"; "blueberry"]
    let longestLength = 
        query {
             for w in words do 
             maxBy w.Length 
        }
    printfn "The longest word is %A characters long." longestLength

    //sample output
    //  The longest word is 9 characters long.


[<Category("LINQ");
  Title("averageBy - Simple");
  Description("This sample uses Average to get the average of all numbers in an array.")>]
let LINQ36() =
    let numbers2 = [5.0; 4.0; 1.0; 3.0; 9.0; 8.0; 6.0; 7.0; 2.0; 0.0]
    let averageNum =
         query { 
            for n in numbers2 do
            averageBy n
         }
    printfn "The average number is %A." averageNum
    
    //sample output
    //  The average number is 4.5.

[<Category("LINQ");
  Title("averageBy - Projection");
  Description("This sample uses Average to get the average length of the words in the array.")>]
let LINQ37() =
    let words = ["cherry"; "apple"; "blueberry"]
    let averageLength = 
        query { 
            for w in words do 
            let wordLength = w.Length |> float
            averageBy wordLength
        }
    printfn "The average word length is %A characters." averageLength

    //sample output
    //  The average word length is 6.666666667 characters.

[<Category("LINQ");
  Title("Concat - Simple");
  Description("This sample uses Concat to create one sequence that contains each array's values, one after the other.")>]
let LINQ38() =
    let numbersA = [0; 2; 4; 5; 6; 8; 9 ]
    let numbersB = [ 1; 3; 5; 7; 8 ]
 
    numbersA.Concat(numbersB)|>Seq.iter(fun n -> printfn "%A" n)


    //sample output
    //    0 
    //    2 
    //    4 
    //    5 
    //    6 
    //    8 
    //    9 
    //    1 
    //    3 
    //    5 
    //    7 
    //    8



[<Category("LINQ");
  Title("SequenceEqual - Simple");
  Description("This sample uses SequenceEqual to see if two sequences match on all elements in the same order.")>]
let LINQ39() =
    let wordsA = [ "cherry"; "apple"; "blueberry" ]
    let wordsB = [ "apple"; "blueberry"; "cherry" ]

    let isMatch = wordsA.SequenceEqual(wordsB)

    printfn "The sequences match: %A" isMatch

    //sample output
    //  The sequences match: false


