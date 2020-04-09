<!-- omit in toc -->
# TJAPlayer3

## Quick Links

- Download the latest release from the [Releases](https://github.com/twopointzero/TJAPlayer3/releases) page.
- Learn more about installation and usage in the [Usage](#usage) section of this document.

<!-- omit in toc -->
## Table of Contents

- [Quick Links](#quick-links)
- [Overview](#overview)
- [Statement Regarding Intellectual Property](#statement-regarding-intellectual-property)
- [Ethos](#ethos)
	- [Goals](#goals)
	- [Non-Goals](#non-goals)
	- [Explicit Denouncements](#explicit-denouncements)
- [Licenses](#licenses)
- [Distribution / Redistribution](#distribution--redistribution)
- [Usage](#usage)
	- [Liability](#liability)
	- [Privacy](#privacy)
	- [System Requirements](#system-requirements)
	- [Installation](#installation)
- [Documentation](#documentation)
- [Support](#support)
- [Issue Tracking](#issue-tracking)
- [Development](#development)
- [Acknowledgements](#acknowledgements)

## Overview

TJAPlayer3 is a rhythm game application for Windows (and compatible environments).

It provides a style of gameplay very similar to that of Taiko no Tatsujin and similar rhythm games, and supports .tja files compatible with a number of other similar rhythm game applications.

TJAPlayer3's default visual appearance bears only a passing resemblance to similar rhythm games but supports skinning which can approximate them if desired.

Per the project [Ethos](#ethos), precise replication of specific commercial or non-commercial rhythm games is an explicit non-goal.

## Statement Regarding Intellectual Property

Any conscious attempts by TJAPlayer3 users to violate any intellectual property are explicitly denounced by the TJAPlayer3 project, its maintainers, and its contributors.

As laws do vary by jurisdiction, it is the sole responsibility of each individual user or developer of TJAPlayer3 to ensure that they adhere to the laws of their legal jurisdiction.

## Ethos

### Goals

- Project sustainability (maintainability, modernization, good open source citizenship, ease of contribution, etc.)
- Satisfying rhythm gameplay, with reliably-accurate judgment
- Quality of life (System, data, and application compatibility, launch time, reliability, song browsing, etc.)
- Priority placed, though not exclusively, on users outside of Japan

Detailed plans regarding the above goals are to be managed in the project roadmap.

As an aside regarding the prioritization of users outside of Japan: With respect, Japanese taiko simulator users have a strong tendency toward chasing precise simulation of the user experience and user interface of modern machines in arcades they can conveniently visit. International users, on the other hand, often struggle to even locate decade-old cabinets (usually in poor condition) within their city, state, or even their entire country. The latter group are underserved and present a set of software requirements which are significantly more sustainable, especially given the limited userbase and development resources interested in such a small niche.

### Non-Goals

- Precise replication of UX, UI, feature-set, or any other aspect of any other commercial or non-commercial rhythm game

### Explicit Denouncements

Taiko simulator development projects have been easily derailed (or at least nearly derailed) by bad actors. TJAPlayer3, as but one example, experienced at least two notable examples within a single calendar year, coinciding with, and essentially halting, its period of greatest development progress.

While the MIT License grants users and developers incredible freedom, _actions performed with that freedom do not receive automatic respect_. One can adhere to the MIT License while being a completely poisonous community member. Such community members are absolutely unwelcome and their behaviour explicitly denounced:

- The TJAPlayer3 contributors explicitly denounce intellectual property violation. Please familiarize yourself with the [Statement Regarding Intellectual Property](#statement-regarding-intellectual-property)
- The TJAPlayer3 contributors explicitly denounce intellectual property hypocricy. For example, if you intend to skin TJAPlayer3 using third-party skin assets, do not complain of `S E C O N D A R Y   D I S T R I B U T I O N` when someone improves on your work or repurposes those assets for something else. We live in a "remix and share" culture, and such hypocricy will absolutely not be tolerated. Go skin some other simulator.
- The TJAPlayer3 contributors explicitly denounce antithetical open source citizenship. For example, if you intend to fork the TJAPlayer3 code, modify it, distribute it privately, and post brag videos on the internet without sharing your code back to the project, then instead go fork some other simulator. Yes, the MIT License allows you to do it. No, we still don't want you around.

The TJAPlayer3 contributors are 100% committed to ensuring the sustainability of the project above all other concerns. If you are offended by any of the above denouncements then you're a part of the problem, not the solution. Kindly redirect your offense to `/dev/null` and go far, far away.

## Licenses

- TJAPlayer3 source code and media assets are licensed under the MIT License. See [/LICENSE](https://github.com/twopointzero/TJAPlayer3/blob/master/LICENSE).
- Bundled libraries are licensed separately, with licenses included in the [/Test/Licenses/](https://github.com/twopointzero/TJAPlayer3/blob/master/Test/Licenses/) folder.

## Distribution / Redistribution

The MIT license allows anyone to do anything with the project's code and assets, including redistributing modified versions and even doing so under the TJAPlayer3 name.
Note that this did in fact happen to TJAPlayer in the past, and was malicious in nature.

To remain safe from malicious parties, only use official releases (or source code) from [twopointzero's TJAPlayer3 GitHub project](https://github.com/twopointzero/TJAPlayer3)

## Usage

### Liability

Though every reasonable attempt is made to ensure that usage of TJAPlayer3 will not adversely impact your computer, no warranty is expressed or implied and your use of the software is entirely at your own risk.

### Privacy

To help make improvements to the software, TJAPlayer3 automatically sends information about software errors to a prominent cloud service called [Sentry](https://sentry.io/).

Sentry maintains strong compliance with privacy regulations in multiple legal jurisdictions. Nevertheless:

- Every effort is made to ensure that TJAPlayer3 collects no personal information in the process of reporting errors.
- Only [twopointzero](https://github.com/twopointzero) has access to these error reports and no access will be granted to others.

If the information above still leaves you feeling uncomfortable, your computer or network firewall software can be configured to block TJAPlayer3 from sending error reports. That said, we do hope that you'll allow it to report errors so that we can all make TJAPlayer3 better together.

### System Requirements

- Windows 7 or greater is required. However...
- Windows 10 is strongly recommended. Correct behaviour on Windows versions lower than 10 is possible but no longer a tested configuration.
- .NET Framework 4.8 is required.
- DirectX 9c is required.

### Installation

To remain safe from malicious parties, only use official release packages or source code directly from [twopointzero's TJAPlayer3 GitHub project](https://github.com/twopointzero/TJAPlayer3).

- Download and decompress an official release package from the [Releases](https://github.com/twopointzero/TJAPlayer3/releases) page.
  - Official release version packages are recommended for general usage
  - Official pre-release version packages can be used if you are comfortable on the leading edge and/or are helping to troubleshoot a reported issue
- Build from source

## Documentation

TJAPlayer3 is currently lacking suitable documentation, as the source fork's documentation was maintained outside of version control and lost upon the abandonment of that project. The documentation will be recreated over time as additions and changes are made to the software.

To find the most current documenation and remain safe from malicious parties, prioritize the use of the official documentation contained within [twopointzero's TJAPlayer3 GitHub project](https://github.com/twopointzero/TJAPlayer3).

[Click here to view the current documentation.](https://github.com/twopointzero/TJAPlayer3/blob/master/docs/index.md)

## Support

Being an open source project supported only by free contribution of time, only a limited amount of support can reasonably be provided. Here are a few constraints one might experience if seeking support from the TJAPlayer3 contributors:

- As the software is used globally, the official language of the TJAPlayer3 project is English, though some accommodation can be provided at times via machine translation
- Only the latest official release package is supported, unless you've been asked to reproduce an issue with a specific pre-release package or by building from source using a specific commit.
- Only the SimpleStyle skin is supported, unless working on a pre-agreed effort to expand skinning capabilities (contact twopointzero.)
- There is no formal monitoring of, or support via, any other application or service (e.g. Discord.)
- If, after troubleshooting, you have isolated a problem to the TJAPlayer3 software itself, please refer to the section below and open a GitHub Issue.

## Issue Tracking

All issues are tracked (and most development managed) using GitHub Issues.

Ensure that you search for an existing issue before opening a new one.

Opened issues will be deleted if the following are not provided promptly upon request: reproduction steps, logs, example .tja files (do _not_ provide media,) example skin config files, etc.

## Development

A full development guide is forthcoming, including information regarding roadmapping of work, semantic versioning of changes, peer review processes, etc. In the meantime, contact twopointzero if you need assistance building from source and/or wish to contribute to development.

## Acknowledgements

A number of individuals created and iterated on the software that came to be TJAPlayer3.

In lieu of attempting to list everyone who has contributed to this and related projects, our thanks go out to the following creators/maintainers of notable members of the TJAPlayer software family:

- FROM/yyagi
- kairera0467
- AioiLight
