# Gemini API Client for .NET and ASP.NET Core
[![GitHub](https://img.shields.io/github/license/starsbane/ai)](https://github.com/starsbane/ai/blob/main/LICENSE)
![GitHub last commit](https://img.shields.io/github/last-commit/starsbane/ai)
[![GitHub stars](https://img.shields.io/github/stars/starsbane/ai)](https://github.com/starsbane/ai/stargazers)

A .NET standard 2.0/.NET 8.0+ library that provides unified interfaces to generate chat, image, embedding, video and speech from text as well as extract text or document intelligence from images across Azure, AWS, Google Cloud and Alibaba Cloud. 

| Package Name | Description | Related Platform Services |
| --- | --- | --- |
| Starsbane.AI | Contains platform-agnostic implementations for PDF text extraction and sentence chunking | N/A |
| Starsbane.AI.Abstractions | Contains platform-agnostic abstractions for the whole project | N/A |
| Starsbane.AI.Alicloud | Provides implementations for Aliyun/Alibaba Cloud | DashScope |
| Starsbane.AI.Amazon | Provides implementations for AWS | AWS Bedrock, Polly, Rekognition, Textract |
| Starsbane.AI.Azure | Provides implementations for Azure | Azure OpenAI, Sora, AI Vision, AI Document Intelligence |
| Starsbane.AI.Google | Provides implementations for GCP | Google Vertex AI, Vision AI, Document AI |

## External package usage
The sentence chunking support is powered by source code from [github.com/GregorBiswanger/SemanticChunker.NET]SemanticChunker.NET, modified for .NET Standard 2.0 support and adapt abstraction in this project.

## To-dos
- Implement tools support for Starsbane.AI.Google
- Better platform and model specific config validation 
- and many more...

## Sample Projects
Please refer to the projects in [samples](samples) folder for the usage.

## Installation
To be updated

## Getting Started
To be updated

##  Author

## License 
This project is licensed under the [Apache-2.0 License](LICENSE) - see the [LICENSE](LICENSE) file for details.
