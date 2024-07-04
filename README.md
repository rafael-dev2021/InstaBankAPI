<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a id="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url] 
[![Youtube][youtube-shield]][youtube-url] 

[![Project Status][Project-Shield]][Project-url]
[![C#][CSharp-Shield]][CSharp-url]
[![.NET][DotNET-Shield]][DotNET-url]
[![LastComit][LastCommit-Shield]][LastCommit-Url]
[![codecov](https://codecov.io/gh/rafael-dev2021/InstaBankAPI/graph/badge.svg?token=WHWXH2Z2B5)](https://codecov.io/gh/rafael-dev2021/InstaBankAPI)
[![GitHub forks](https://img.shields.io/github/forks/rafael-dev2021/InstaBankAPI?style=social)](https://github.com/rafael-dev2021/InstaBankAPI/network/members)
[![GitHub stars](https://img.shields.io/github/stars/rafael-dev2021/InstaBankAPI?style=social)](https://github.com/rafael-dev2021/InstaBankAPI/stargazers)

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/rafael-dev2021/InstaBankAPI">
    <img src=".github/images/logo.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">InstaBankAPI</h3>

  <p align="center">
    A digital banking microservice
    <br />
    <a href="https://github.com/rafael-dev2021/InstaBankAPI"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/rafael-dev2021/InstaBankAPI">View Demo</a>
    ·
    <a href="https://github.com/rafael-dev2021/InstaBankAPI/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/rafael-dev2021/InstaBankAPI/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://www.linkedin.com/in/rafael-s-a79314207)

<p>In short, the main objective of the project is to provide a secure API for user authentication and another for managing banking operations, both with robust validation and protection through authentication and authorization.</p>

<h3>AuthenticateAPI</h3>

 * Purpose: To provide authentication and authorization services.
 * Features: Include login, registration, password reset, logout, profile update, and token verification.
 * Security: Uses authentication and authorization to protect endpoints, validating JWT tokens and ensuring that only authorized users can access certain operations.

<h3>BankingServiceAPI</h3>

 * Purpose: To manage banking operations such as account creation, deposits, transfers and withdrawals.
 * Functionalities: These include creating bank accounts, checking accounts, deposits, transfers between accounts and withdrawals.
 * Security: Endpoints protected by authentication to ensure that only authenticated users can carry out banking operations.

<h3>Common Features</h3>

 * Use of DTOs (Data Transfer Objects): To standardize requests and responses.
 * Request Validation: Use of FluentValidation to validate input data.
 * Exception handling: Includes mechanisms for handling unauthorized access exceptions and other failures.
<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With

<p>The project uses C# and .NET to develop authentication APIs and banking services, with SQL Server for the database, Redis for caching, Docker to manage containers, xUnit for unit tests, Serilog for logging, and JWT for secure authentication.</p>

* [![Csharp][Csharp.io]][Csharp-url]
* [![XUnit][XUnit.com]][XUnit-url]
* [![Jwt][Jwt.com]][Jwt-url]
* [![Dotnet][Dotnet.io]][Dotnet-url]
* [![Serilog][Serilog.com]][Serilog-url]
* [![Redis][Redis.dev]][Redis-url]
* [![Docker][Docker.com]][Docker-url]
* [![Sql][Sql.io]][Sql-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

Software you must have installed on your local machine

 * Docker desktop [Download docker desktop](https://www.docker.com/products/docker-desktop/)
 * Rider (IDE) [Download Rider IDE](https://www.jetbrains.com/pt-br/rider/download/#section=windows)
 * Visual Studio 2022 [Download Visual Studio](https://visualstudio.microsoft.com/pt-br/downloads/)

### Installation

#### 1. Clone the repository
   ```sh
   git clone https://github.com/rafael-dev2021/InstaBankAPI.git
   ```
#### 2. In the InstaBankAPI project, change the project location to AuthenticateAPI <br>
  * `Cd AuthenticateAPI` <br>
  * Create a file `.env` and add these environment variables <br>
   ```sh
   DB_PASSWORD=YourStrong!Passw0rd
   SECRET_KEY= generate a 64-bit HMAC secret key and paste it here
   ISSUER= your issuer here
   AUDIENCE= your audience here
   EXPIRES_TOKEN=15
   EXPIRES_REFRESHTOKEN=30
   OPENAPI_URL=https://github.com/rafael-dev2021
   REDIS_CONNECTION=localhost:6379
   ```
  
  * After the `.env` file has been added, now go to the terminal in the same AuthenticateAPI project and add this command <br>
   ```sh
   Docker-compose up --build
   ```
   or
   ```sh
   Docker-compose up -d
   ```
   
  * Add the migrations of the AuthenticateAPI project in the terminal with these commands <br>
   ```sh
   Dotnet ef migrations add Name of your table
   ```
   after
   ```sh
   Dotnet ef database update
   ```
<hr>

#### 3. In the InstaBankAPI project, change the project location to BankingServiceAPI <br>
  * `Cd BankingServiceAPI` <br>
  * Create a file `.env` and add these environment variables <br>
   ```sh
   DB_PASSWORD=YourStrong!Passw0rd
   SECRET_KEY= the same secret key as AuthenticateAPI here
   ISSUER= same issuer as the AuthenticateAPI here
   AUDIENCE= the same audience as the AuthenticateAPI here
   OPENAPI_URL=https://github.com/rafael-dev2021
   REDIS_CONNECTION=localhost:6380
   BASE_URL=https://localhost:7074
   ```
  * After the `.env` file has been added, now go to the terminal in the same BankingServiceAPI project and add this command <br>
   ```sh
   Docker-compose up --build
   ```
   or
   ```sh
   Docker-compose up -d
   ```
  * Add the migrations of the BankingServiceAPI project in the terminal with these commands <br>
   ```sh
   Dotnet ef migrations add Name of your table
   ```
   after
   ```sh
   Dotnet ef database update
   ```
<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

My Discord:
```sh
 Rafael98k#9440
```
Email to contact: 
```sh
 rafael98kk@gmail.com
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/rafael-dev2021/InstaBankAPI.svg?style=for-the-badge
[contributors-url]: https://github.com/rafael-dev2021/InstaBankAPI/graphs/contributors
[issues-shield]: https://img.shields.io/github/issues/rafael-dev2021/InstaBankAPI.svg?style=for-the-badge
[issues-url]: https://github.com/rafael-dev2021/InstaBankAPI/issues
[license-shield]: https://img.shields.io/github/license/rafael-dev2021/InstaBankAPI.svg?style=for-the-badge
[license-url]: https://github.com/rafael-dev2021/InstaBankAPI/blob/main/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white
[linkedin-url]: https://www.linkedin.com/in/rafael-s-a79314207/
[youtube-shield]: https://img.shields.io/badge/YouTube-FF0000?style=for-the-badge&logo=youtube&logoColor=white
[youtube-url]: https://www.youtube.com/channel/UC9A-6w3A_GRs5rp8ct_1OlA
[product-screenshot]: .github/images/screenshot.png
[Csharp.io]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white
[Csharp-url]: https://learn.microsoft.com/pt-br/dotnet/csharp/
[Dotnet.io]: https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[Dotnet-url]: https://dotnet.microsoft.com/
[Sql.io]: https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white
[Sql-url]: https://learn.microsoft.com/en-us/sql/sql-server/?view=sql-server-ver16
[Redis.dev]: https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white
[Redis-url]: https://redis.io/docs/latest/
[Docker.com]: https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white
[Docker-url]: https://docs.docker.com/desktop/
[XUnit.com]: https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=xunit&logoColor=white
[XUnit-url]: https://xunit.net/
[Serilog.com]: https://img.shields.io/badge/Serilog-303030?style=for-the-badge
[Serilog-url]: https://serilog.net/ 
[Jwt.com]: https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=JSON%20web%20tokens
[Jwt-url]: https://jwt-auth.readthedocs.io/en/develop/
[Project-Shield]: https://img.shields.io/badge/status-under%20development-green
[Project-url]: https://github.com/rafael-dev2021/InstaBankAPI
[CSharp-Shield]: https://img.shields.io/badge/C%23-12.0-green
[DotNET-Shield]: https://img.shields.io/badge/.NET-8.0-512BD4
[LastCommit-Shield]: https://img.shields.io/github/last-commit/rafael-dev2021/ECommerce?color=green
[Lastcommit-Url]: https://github.com/rafael-dev2021/InstaBankAPI/commits/main/ 
