## v5.9.0 (minor)

Changes since v5.8.0:

- [minor] Keep the major third in a b5 chord unless a minor third is spelled ([@Claude](https://github.com/Claude))
- Parse CMaj7, CmMaj7 and C°7 as the sevenths they name [patch] ([@Claude](https://github.com/Claude))
- Move CI onto the shared ci-shared.yml pipeline ([@Claude](https://github.com/Claude))
- Match path containment case to the platform and accept paths directly under a root [patch] ([@matt-edmondson](https://github.com/matt-edmondson))
- Parse "ionian" as the major mode, so a parsed Ionian key equals C major [patch] ([@matt-edmondson](https://github.com/matt-edmondson))
- Say why the quality-word loop body is empty [patch] ([@Claude](https://github.com/Claude))
- Strip the quality words without the netstandard2.0-missing Replace overload [patch] ([@Claude](https://github.com/Claude))
- Read C69, Cadd11 and Cadd13 as added tones, and reject leftover text [patch] ([@Claude](https://github.com/Claude))
- Let AdjustForContrast darken on mid-tone backgrounds [patch] ([@Claude](https://github.com/Claude))

