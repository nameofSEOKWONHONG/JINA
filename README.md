# JINA
**Just Do it**, **I**nfrastructure for .**N**et **A**pplication

## JINA란?
* JINA는 ASP.NET CORE+ 에 적용되는 공통 라이브러리 및 추상화 계층의 서버측 구현입니다.
* JINA는 기본적으로 CQRS에 대한 독자적 구현을 포함합니다.
* JINA는 CQRS 구현에 필요한 CACHE, 다국어, Validation, Context 패턴에 대한 구현이 포함됩니다.

## 상세
* Jina.Excel
    * Excel에 대한 구현입니다. ClosedXml을 사용합니다.
* Jina.Lang
    * 지역화 언어에 대한 구현입니다. Json을 기초로 합니다.
* Jina.Sql
    * Jint를 이용한 Sql Plain Text Query의 동적 구현입니다.
    * BulkInsert를 포함합니다.
* Jina.Validate
    * FluentValidation의 확장입니다.
    * Validation은 Jina.Lang과 결합되어 다국어를 지원합니다.
* Jina.SequenceGenerator ``` 언제든 제거 될 수 있습니다. ```
    * EF를 이용한 수동 채번에 대한 구현입니다.

## 주의
* Jina.Lang, Jina.Validate는 Blazor SPA Client를 고려하여 Jina에 대한 의존성이 없습니다.
* Jina는 Jina.Lang, Jina.Validate의 의존성이 포함됩니다.
* 공통화 시나리오(Session, Context)에 따라 Excel, Sql은 Jina의 의존성이 포함됩니다.

## Thank you to

[JetBrains](https://www.jetbrains.com/?from=Jina) kindly provides `Jina` with a free open-source licence for their Resharper and Rider.
- **Resharper** makes Visual Studio a much better IDE
- **Rider** is fast & powerful cross platform .NET IDE

<img src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.png" alt="JetBrains Logo (Main) logo." style="width:200px;height:200px;">
