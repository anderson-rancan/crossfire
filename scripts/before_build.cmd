nuget.exe restore CrossFire.sln

choco install "msbuild-sonarqube-runner" -y

MSBuild.SonarQube.Runner.exe begin /k:"CrossFire" /d:"sonar.host.url=https://sonarqube.com" /d:"sonar.login=%SONAR_LOGIN%" /d:sonar.cs.opencover.reportsPaths="C:\projects\crossfire\coverage.xml" /d:"sonar.organization=anderson-rancan-github"