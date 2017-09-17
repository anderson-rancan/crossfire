# CrossFire blog projects

This repository contains the code for every post/article from the <a href="https://arancan.wordpress.com/">CrossFire blog</a>.

[![Build status](https://ci.appveyor.com/api/projects/status/1sm6e4iv9ixhmxhy?svg=true)](https://ci.appveyor.com/project/anderson-rancan/crossfire)
[![MIT licensed](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/anderson-rancan/crossfire/blob/master/LICENSE)
[![Coverage Status](https://coveralls.io/repos/github/anderson-rancan/crossfire/badge.svg?branch=master)](https://coveralls.io/github/anderson-rancan/crossfire?branch=master)
<a href="https://scan.coverity.com/projects/anderson-rancan-crossfire"><img alt="Coverity Scan Build Status" src="https://scan.coverity.com/projects/13794/badge.svg"/></a>
<a href="https://sonarqube.com/dashboard/index?id=CrossFire"><img src="https://sonarqube.com/api/badges/gate?key=CrossFire" alt="SonarQube quality gate"></a>
<a href="https://sonarqube.com/dashboard/index?id=CrossFire"><img src="https://sonarqube.com/api/badges/measure?key=CrossFire&metric=code_smells" alt="SonarQube code smells"></a>


## ProducerConsumer project

This project demonstrates a simple easy-to-use implementation of the producer-consumer pattern:
* one task to consume content
* one or more tasks to produce content
* appliance of <a href="https://msdn.microsoft.com/en-us/library/dd267312(v=vs.110).aspx">BlockingCollection</a> between tasks
