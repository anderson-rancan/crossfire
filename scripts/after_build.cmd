C:\projects\crossfire\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user "-target:C:\projects\crossfire\packages\NUnit.ConsoleRunner.3.7.0\tools\nunit3-console.exe" "-targetargs: C:\projects\crossfire\Tests\MultithreadProducerConsumerUnitTest\bin\Debug\MultithreadProducerConsumerUnitTest.dll" -output:C:\projects\crossfire\coverage.xml -returntargetcode

C:\projects\crossfire\packages\ReportGenerator.3.0.0\tools\ReportGenerator.exe "-reports:C:\projects\crossfire\coverage.xml" "-targetdir:C:\projects\crossfire\.coverage"

for /f %%i in ('git rev-parse HEAD') do set commit_id=%%i
for /f %%i in ('git rev-parse --abbrev-ref HEAD') do set commit_branch=%%i
for /f "delims=" %%i in ('git log -1 --pretty^=%%B') do set commit_msg=%%i
for /f "delims=" %%i in ('git log -1 --pretty^=%%an') do set commit_author=%%i
for /f %%i in ('git log -1 --pretty^=%%ae') do set commit_email=%%i

C:\projects\crossfire\packages\coveralls.net.0.7.0\tools\csmacnz.Coveralls.exe --opencover -i C:\projects\crossfire\coverage.xml --repoToken $env:%COVERALLS_TOKEN% --commitId %commit_id% --commitBranch %commit_branch% --commitAuthor %commit_author% --commitEmail %commit_email% --commitMessage %commit_msg%

MSBuild.SonarQube.Runner.exe end /d:"sonar.login=%SONAR_LOGIN%"