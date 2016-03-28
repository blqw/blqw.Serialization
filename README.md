# blqw.Serialization
任意对象的序列化和反序列化


####再也不需要受限于系统的``[Serializable]``特性  
* 理论上将任何实例对象都序列化和反序列化  
* 适用场景一,分布式数据存储,如:Cache,Session,各种分布式数据库在不同服务器间的数据传递  
* 适用场景二,服务器日志将数据序列化后存入日志仓库,反序列化后不会丢失任何内容  

## 欢迎测试,如果有无法序列化或反序列化的类型,欢迎提交BUG  

### 开发计划
* 完善文档注释    
* 反序列化类型不存在时,构造一个动态类型  
* 序列化时可以选择策略,高压缩,高性能,平衡  

### 更新日志  
#### 2016.03.29
* 重构代码  

#### 2016.03.16  
* 更新MEF  
  
#### 2016.03.14  
* 优化序列化成字符串的逻辑  
  
#### 2016.03.08
* 增加序列化流文件的描述头信息  
* 修复bug  

#### 2016.03.01  
* 输出调试信息功能完成  
* 简单单元测试  
* 修复密封类为null时的序列化报错bug  
* 支持匿名类型的序列化  
* MEF插件输出测试完成

#### 2016.02.29  
* 一维/多维数组 格式器编码完成  
* 循环引用 格式器编码完成  
* gzip 转 string 完成   
* 元类型 格式器编码完成   
* IntPtr,UIntPtr,Uri,NameValueCollection 格式器编码完成  
* 正式更名为 blqw.Serialization (更贴近微软的命名方式)  
* 老项目[blqw.Serializable](https://github.com/blqw/blqw.Serializable)停止维护  

#### 2016.02.28
* 项目重构,现在有一种更好的序列化方案(性能提升,兼容性极大的提升,序列化体积减少,代码逻辑优化)
* 努力Coding...  
