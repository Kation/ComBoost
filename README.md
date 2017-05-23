ComBoost 3.0
========

ComBoost 3.0基于.Net Core框架进行开发

3.0开始将从实体框架转变为服务框架，不再仅限于实体类目标。

项目包含程序集：
* Wodsoft.ComBoost  
包含领域服务的一些基础接口实现，例如DomainContext，DomainService，DomainProvider等。用于领域服务项目。
* Wodsoft.ComBoost.AspNetCore  
包含基于AspNetCore的Http领域上下文实现。
* Wodsoft.ComBoost.AspNetCore.Security  
包含Asp.Net Core的身份认证模块功能，方便开发者进行身份验证。用于非Mvc网站项目，一般不直接引用。
* Wodsoft.ComBoost.Core  
包含框架核心接口定义与接口的扩展方法。一般不直接引用。
* Wodsoft.ComBoost.Data  
包含实体领域服务。用于网站项目。
* Wodsoft.ComBoost.Data.Core  
包含实体领域的接口定义、基础实现与通用扩展方法等。用于实体层项目。
* Wodsoft.ComBoost.EntityFramework
包含Entity Framework 6.x的实现。用于网站项目。
* Wodsoft.ComBoost.EntityFrameworkCore  
包含Entity Framework Core 1.x 的实现。用于网站项目。
* Wodsoft.ComBoost.Mock  
包含领域服务单元测试的模拟模块，可以方便的编写领域服务的单元测试。用于单元测试项目。
* Wodsoft.ComBoost.Mvc  
包含领域控制器，领域视图组件，Mvc领域上下文等基础类型，方便开发者调用领域服务。用于Mvc网站项目。
* Wodsoft.ComBoost.Mvc.Data  
包含实体领域控制器，方便开发者编写增删查改。用于Mvc网站项目。
* Wodsoft.ComBoost.Redis  
包含使用Redis的缓存、锁的实现。用于领域服务项目。
* Wodsoft.ComBoost.Security  
包含框架权限核心模块。一般不直接引用。
* Wodsoft.ComBoost.Storage  
包含使用本地物理文件实现的储存提供器。一般用于领域服务项目。