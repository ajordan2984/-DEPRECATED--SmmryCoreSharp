# SmmryCoreSharp

This is a library written in C# for http://smmry.com/

## Credits
https://github.com/ghostiek/SmmrySharp

## Updates
* Updated from netstandard2.0 to netcoreapp3.1.
* Updated error checking to block null, empty, or whitespace parameters in the: Api, Url, and either both Url and Text or neither one.
* Added in **sm_api_input** to SmmryParameters.cs.
    * **sm_api_input** allows a "Text" parameter so that a block of text can be summarized instead of passing a URL to SMMRY.

## Dependencies
Dependency        | Version
----------------- | -------------
.NET Core         | 3.1
Newtonsoft        | 12.0.3

## Example

This is how you would make your request
```cs
var client = new SmmryClient(new SmmryParameters()
            {
                ApiKey = "YOUR API KEY HERE",
                Url = "https://en.wikipedia.org/wiki/Augustus",
                SentenceCount = 3,
                KeywordCount = 24,
                IncludeBreaks = false,
                IncludeQuotes = false
            });
            
var smmry = client.Download();
```
Note: Every parameter is optional except your API key and website URL