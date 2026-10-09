## v5.11.4 (patch)

Changes since v5.11.3:

- Build expected paths with Path.Join in RelativePathMakeTests ([@Claude](https://github.com/Claude))
- [patch] Compute RelativePath.Make by comparing paths, not URIs ([@Claude](https://github.com/Claude))
- [patch] Keep NormalizedParameter.Normalize in [0, 1] for out-of-domain values and reject empty ranges ([@Claude](https://github.com/Claude))
- [patch] Accept only one declared SectionType name in a section header ([@Claude](https://github.com/Claude))
- [patch] Reject JWT segments outside the unpadded base64url alphabet ([@Claude](https://github.com/Claude))
- [patch] Stop [PrefixAndSuffix] letting the prefix and suffix overlap, and reject empty strings ([@Claude](https://github.com/Claude))
- [patch] Require the 978/979 Bookland prefix on an ISBN-13 ([@Claude](https://github.com/Claude))
- Filter IBAN whitespace with Where instead of an if inside the loop ([@Claude](https://github.com/Claude))
- [patch] Anchor IBAN, UUID and ULID patterns at the true end, and strip all whitespace from an IBAN ([@Claude](https://github.com/Claude))
- [patch] Strip every trailing separator when canonicalizing a path ([@Claude](https://github.com/Claude))
- [patch] Stop Pitch octaves wrapping into range, and reject whitespace or '+' in the octave ([@Claude](https://github.com/Claude))
- [patch] Keep Tempo.TryParse from throwing, and reject infinite tempos and non-positive beats ([@Claude](https://github.com/Claude))
- [patch] Name the aeolian mode once ([@Claude](https://github.com/Claude))
- [patch] Parse "minor" and "natural_minor" as the Aeolian mode ([@Claude](https://github.com/Claude))

