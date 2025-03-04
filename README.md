# Pdf Analyser

Pdf Analyser is a tool to convert PDF files to CSV.

## Requirements

.NET8.0 Runtime on Windows or Linux

## Configuration

A YAML file defines what PDF files should be parsed and how the content is structured. Runnable examples can be found in the samples folder. 

I'd recommend to export a sample file as txt. Then go to https://regex101.com/ and try to figure out the best matching patterns for your files.

## Usage

You can start OCSoft.PdfAnalyser.exe from the command line like so.

```sh
	OCSoft.PdfAnalyser.exe statements.yaml 
```

A smarter way for windows users: 
You do not have to name your file .yaml but you can choose whatever file extension you like. This allows you to associate the extension with the OCSoft.PdfAnalyser.exe and automatically export all PDF data as one CSV file.

## Evaluation
You can import the CSV into Power BI for further evaluation or into [KYSA](https://digital-souveraenitaet.de/kysa-know-your-spendings/).

## License

Pdf Analyser is licensed under the Clear BSD License. See License.txt for details.

## Donation

Buy me a coffee [![buy me a coffee](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExODE1ZjhjOWNiYjczZjE3ZjY3ZDMwZTRlNjI0MGNlZjg0ZGEyYmQzNSZjdD1z/RETzc1mj7HpZPuNf3e/giphy.gif)](http://buymeacoffee.com/ortwic)