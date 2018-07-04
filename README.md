邮件朗读 for MailBird
=======

[![Build status](https://ci.appveyor.com/api/projects/status/ol2vkhpx34k8cycu?svg=true)](https://ci.appveyor.com/project/wzv5/mailtts)

由于 [MailBird] 还挺好用的，但不开放插件接口，而我又想要新邮件自动朗读功能，于是就写了这个。

目标是以最小的侵入来实现功能，起初还想注入个dll进去，但是在研究 MailBird 程序结构时发现，数据库位置是固定的，并且没有加密，可以直接读取数据库来获取最新邮件。

至于新邮件送达的事件，直接监视数据库文件改动就行了。

完美！

我真是太强了！

使用说明：
--------

安装 MailBird 并添加邮件帐号后，直接运行 `MailTTS.exe`，一旦有新邮件送达，会自动调用系统默认TTS引擎来朗读标题和发件人。

win10 内置了“Huihui”语音，可以直接调用，win7 或其他精简版本系统，可能需要手动安装 TTS 语音包。

下载：
-----

<https://github.com/wzv5/MailTTS/releases/latest>

[MailBird]: https://www.getmailbird.com/r/343500