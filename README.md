# CrossFire blog projects

This repository contains the code for every post/article from the <a href="https://arancan.wordpress.com/">CrossFire blog</a>.

[![Build status](https://ci.appveyor.com/api/projects/status/1sm6e4iv9ixhmxhy?svg=true)](https://ci.appveyor.com/project/anderson-rancan/crossfire)
[![MIT licensed](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/anderson-rancan/crossfire/blob/master/LICENSE)
[![Coverage Status](https://coveralls.io/repos/github/anderson-rancan/crossfire/badge.svg?branch=HEAD)](https://coveralls.io/github/anderson-rancan/crossfire?branch=HEAD)
[![Quality Gate](https://sonarcloud.io/api/badges/gate?key=CrossFire)](https://sonarcloud.io/dashboard/index/CrossFire)
<a href="https://sonarcloud.io/project/issues?id=CrossFire&resolved=false"><img src="https://sonarcloud.io/api/badges/measure?key=CrossFire&metric=open_issues" alt="SonarQube Open issues"></a>

## ProducerConsumer project

This project demonstrates a simple easy-to-use implementation of the producer-consumer pattern:
* one task to consume content
* one or more tasks to produce content
* appliance of <a href="https://msdn.microsoft.com/en-us/library/dd267312(v=vs.110).aspx">BlockingCollection</a> between tasks
