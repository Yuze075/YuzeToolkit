# Changelog

此项目的所有显著更改都将记录在此文件中。

该格式基于[Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)
，并且此项目遵循[语义版本控制](https://semver.org/lang/zh-CN/)。

## [Unreleased]

### Added

- 导入了`SessionManager` `SceneManagement` `LagCompensation`, 待具体了解和实现 // todo

### Changed

- 优化`IocTool`将`Container`模块从`MonoBehaviour`中独立处理 // todo
- 完全区分`ShowValue`的使用(具体研究一下Unity的序列化机制, 避免产生对Runtime的影响) // todo

### Deprecated

### Removed

### Fixed

### Security

## [0.9.0] - 2023-2-18

### Added

- 完整的游戏工具集合, 详细查看[README](README.md)
