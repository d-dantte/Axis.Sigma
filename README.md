# Axis.Sigma
> Simplified Attribute Based Access Control.

## TOC


## Policy Rule Expressions

Sigma boasts an expression language used for representing policy rules, `SPREE` - Sigma Policy Rule
Enforcement Expressions. `SPREE` syntactically resembles regular c-type language script, with a few
additions to help with readability.

Internally, `SPREE` operates on an `AccessContext` instance - this instance is exposed to user
script as a 4 global references, each representing one of the 4 properties of the `AccessContext`
type. Given below is a detailed explanation of the components of `SPREE`.


### Global References Expression

The 4 global references are `@subject`, `@intent`, `@resource`, and `@environment`. When used in
isolation, the references return the `string` ID of the entity they represent. All except for the
`@intent` reference can further be drilled into to access the underlying attribtues.


### Attribute Access Expression

Accessing attributes of a global reference comes in two flavors:
1. Unit access: `@subject.AttributeName`, or `@subject.'Attribute Name'`. This gets the value of
   the first attribute found within the entity whose name matches "AttributeName". The second
   example shows how to expressattribute names that contain any character outside of the
   regular `\w` characters.
2. Set access: `@subject[AttributeName]`, or `@subject['Attribute Name']`. This returns a set of
   a values of all attributes whose names are equal to "AttributeName". As with #1 above, when
   the attribute name contains non-word characters, it is wrapped in a `'`. There is a third
   form of attribute set access: `@subject[\pattern\]`. This returns a set of values of all
   attributes whose names match the given regex pattern. This set of values must be homogeneous
   - if any of the matched attributes has a value-type different from what is in the set, an
   empty set is returned.

> __NOTE__: In the context of "unit access", if the attribute does not exist, the entire expression
evaluates to false.

### Property Access Expression

Some values possess properties. These properties can be accessed to return their values, and these
values will in turn participate in the expression evaluation. Accessing properties is done same as
with the [global references](#global-references-expression), e.g:
```
@environment.Now.DayOfWeek
```
The above example demonstrates accessing the `DayOfWeek` property of the `Now` attribute of the
`@environment` reference.

Types with properties include:
1. `global-ref`. All attributes of the entity are expressed via properties as described
   [above](#attribute-access-expression).
2. `set`
   - `Length`: the number of elements in the set.
3. `Duration`
   - `Ticks`: the tick component of the duration
   - `Seconds`: the seconds component of the duration
   - `Minutes`: the minutes component of the duration
   - `Hours`: the hours component of the duration
   - `Days`: the days component of the duration
   - `TotalXXX`: e.g, `TotalTicks`, etc. This returns the entire duration expressed as one of it's
     components. e.g, if a duration was exactly 1 day, the `TotalHours` will be 24, the
     `TotalSeconds` will be 86400, etc.
4. `Timestamp`. All properties are `int` values, except for `TimeZone`.
   - `Ticks`: the tick component of the timestamp
   - `Seconds`: the seconds component of the timestamp
   - `Minutes`: the minutes component of the timestamp
   - `Hours`: the hours component of the timestamp
   - `TimeZone` -> `Duration`: the timezone component of the timestamp
   - `DayOfWeek`: the day-of-week component of the timestamp, a value between 1 and 7 (inclusive).
   - `DayOfMonth`: the day-of-month component of the timestamp, a value between 1 and 31
     (inclusive), depending on the month.
   - `Month`: the month component of the timestamp
   - `Year`: the year component of the timestamp


### Constant Value Expression

`SPREE` supports 10 types - 2 integer-numeric types, 3 decimal-numeric types, `Boolean`,
`Character`, `string`, `Timestamp`, and `Duration`. Detailed below are the rules for expressing
constant values of the types.

#### Boolean
The boolean type has only two values, expressed as follows:
```
true
false
```

#### Character
The character type is expressed as a single character surrounded by `'`. e.g
```
'x'
'&'
```
NOTE: this should not be confused with the [attribute access](#attribute-access-expression)
expression that is ALWAYS preceeded by a `.`, whereas the character expression isn't.

#### Integer
There are two integer types - `int`, corresponding to the `System.Long` type, and the `bigint`,
corresponding to the `System.Numerics.BigInteger` type. The constant expressions of these types
are distinguishable by the mandatory suffix attached to the value. `int` has a `i` suffix, while
`bigint` has a `I` suffix.
```
5443I
-1220001I
0000000001i
-0i
```

#### Real
This corresponds to the `System.Double` type. Real numbers are expressed using either regular
decimal notation, or the scientific notation. With either of these options, the number must be
suffixed with an `r`.
```
0.0r
-3e23r
2.001e-6r
```

#### Decimal
There are two decimal types - `decimal`, corresponding to the `System.Double` type, and the
`bigdecimal`, corresponding to the `Axis.Luna.Common.Numerics.BigDecimal` type. Decimals are
expressed using either regular decimal notation, or the scientific notation. As with integer
values, decimals have a mandatory suffix, `d` for `decimal`, and `D` for `bigdecimal`.
```
0.0d
-3e23D
2.001e-6d
```

#### String
The string expressed exactly as a stirng in `c#`.
```
"the quick brown fox...etc"
"" // empty string
```

#### Timestamp
Corresponds to the `System.DateTimeOffset` type. It's constant expression is as follows:
```
'T 2023-02-22 19:08:37.9883021 +04:00'
'T 1965-06-24 22:00:07'
'T 2003'
```

With decreasing precision, all missing components assume defualt values. E.g, for the example
`'T 1965-06-24 22:00:07'`, the millisecond/tick component of the time assumes `0` as it's
value. Again, the `'T 2003'` timestamp assumes `1st January, 00:00:00.0` for the missing values.
The highest precision is a call to the date-time's `ToString(..)` method with the following
format: `"yyyy-MM-dd HH:mm:ss.fffffff zzz"`

#### Duration
Corresponds to the `System.TimeSpan` type.
```
'D 23:2:43:38.004332'
'D 12:00:00:00.0000000'
```
Unlike [timestamp](#timestamp), `duration` always uses full precision.

#### Range expression
Ranges are used to define a lower and upper inclusive limit to test another value against. They
are only ever used with the [between](#between-expression) expression.

Syntax: `[<lhs>..<rhs>]`
```
[1i..50i]
['T 1980'..'T 2020']
```
##### Supported operand types:
All types except `bool` are supported. Note that the `<lhs>` and `<rhs>` MUSt be of the exact same
type.

#### Set expression
Sets are used to define a set of values to test intersection, exclusion, or subsets. The subset
must have at least 2 operands, separated by a comma, and no upper limit of operands.

Syntax: `[<operand>, <operand>, <operand> ,...]`

Examples:
```
[2, 1, 66, 4]
['T 1980', 'T 1981']
```
##### Supported operand types:
All types are supported. The set expression is type-homogeneous, meaning, like ranges, the operands
MUSt all be of the same type.


### Unary Expressions

Unary expressions are a combination of an operator, and a single operand. For some cases, the
operan appears before the operator, and in other cases, it appears after.

#### Present Expression
This expression operates ONLY on [attribute access expressions](#attribute-access-expression).
It returns a boolean value, indicating the presence or absence of an attribute.

Syntax: `<operand> present`

Examples:
```
@subject.Department present
@resource.Volume present
@environment.Now present
```

#### Absent Expression
This expression operates ONLY on [attribute access expressions](#attribute-access-expression).
It returns a boolean value, indicating the absence or presence of an attribute.

Syntax: `<operand> absent`

Examples:
```
@subject.Department absent
@resource.Volume absent
@environment.Now absent
```

#### Not Expression
This operates ONLY on `boolean expressions`. It flips the boolean value of the operand. The
operand is whatever expression that immediately appears after the operator.

Syntax: `not <operand>`

Examples:
```
not true
not (@subject.Age > 5)
not @resource.IsExpired
```

#### as string Expression
Operates on all types except `string`. Converts the value to a string representation.

Syntax: `<operand> as string`

Examples:
```
true as string //equivalent to "true"
@subject[Department] as string // converts the set to a string. of the form "['value1', 'value2',...]"
@environment.Now as string // calls the "ToString" of the Datetime with the format "yyyy-MM-dd HH:mm:ss.fffffff zzz"
```

#### () Expression
The brackets are used to group an expression, usually in the context of
[polynary expressions](#polynary-expressions), to delimit an expression that may otherwise have
been interpreted differently because of operator precedence. Outside of the context of
polynary-expressions, the bracket or group operator simply delimits the expression it encapsulates.

Syntax: `(<operand-expression>)`

Any expression can be used as an operand for the bracket expression.

### Binary Expressions

A combination of an operator, and 2 operands. The arrangement of the trio is dependent on the
expression.

#### & Expression
Used for expressing the `and` binary-algebra operation.

Syntax: `<lhs-operand> & <rhs-operand>`

Examples:
```
@resource.Flags & 64
```

##### Supported operand types:
1. `<integer> & <integer>` -> `integer` : supports all combinations of `bigint` and `int` values.
   If either of the operands is a `bigint`, the result is `bigint`, else it is an `int`.
2. `<bool> & <bool>` -> `bool`.

#### | Expression
Used for expressing the `or` binary-algebra operation.

Syntax: `<lhs-operand> | <rhs-operand>`

Examples:
```
@resource.Flags | 64
```

##### Supported operand types:
1. `<integer> | <integer>` -> `integer` : supports all combinations of `bigint` and `int` values.
   If either of the operands is a `bigint`, the result is `bigint`, else it is an `int`.
2. `<bool> | <bool>` -> `bool`.

#### ~ Expression
Used for expressing the `xor` binary-algebra operation.

Syntax: `<lhs-operand> ~ <rhs-operand>`

Examples:
```
@resource.Flags ~ 64
```

##### Supported operand types:
1. `<integer> ~ <integer>` -> `integer` : supports all combinations of `bigint` and `int` values.
   If either of the operands is a `bigint`, the result is `bigint`, else it is an `int`.
2. `<bool> ~ <bool>` -> `bool`.

#### and Expression
Used for expressing the conditional `and` operation. Like in programming languages, this evaluates
to true if both the `lhs` and `rhs` are true.

Syntax: `<lhs> and <rhs>`

Examples:
```
@resource.IsGood and @resource.IsGreat
```

##### Supported operand types:
1. `<bool> and <bool>` -> `bool`

#### or Expression
Used for expressing the conditional `or` operation. Like in programming languages, this evaluates
from left to right, returning true at the first operand that evaluates to true.

Syntax: `<lhs> or <rhs>`

Examples:
```
@resource.IsGood and @resource.IsGreat
```

##### Supported operand types:
1. `<bool>` and `<bool>` -> `bool`


#### + Expression
It's meaning is dependent on the types of the `<lhs>` and `<rhs`.

Syntax: `<lhs> + <rhs>`

Examples:
```
45 + 6
'T 2018-02-13 12:04:19' + 'D 01:23:12'
```

##### Supported operand types:
1. `<number> + <number>` -> `number`. Addition. Any combination of numbers can be used as operands,
   but the result is lifted to the highest numeric type.
2. `<string> + <stirng>` -> `string`. Concatenation.
3. `<Timestamp> + <duration>` -> `Timestamp`. Adds a duration to a timestamp.
4. `<Duration> + <Timestamp>` -> `Timestamp`. Adds a duration to a timestamp.
5. `<Duration> + <Duration>` -> `Duration`. Adds a duration to another duration.


#### - Expression
It's meaning is dependent on the types of the `<lhs>` and `<rhs`.

Syntax: `<lhs> - <rhs>`

Examples:
```
45 - 6
'T 2018-02-13 12:04:19' - 'D 01:23:12'
```

##### Supported operand types:
1. `<number> - <number>` -> `number`. Subtraction. Any combination of numbers can be used as operands,
   but the result is lifted to the highest numeric type.
3. `<Timestamp> - <duration>` -> `Timestamp`. subtracts a duration from a timestamp.
4. `<Duration> - <Timestamp>` -> `Timestamp`. subtracts a duration from a timestamp.
5. `<Duration> - <Duration>` -> `Duration`. subtracts a duration from another duraiton.


#### * Expression
It's meaning is dependent on the types of the `<lhs>` and `<rhs`.

Syntax: `<lhs> * <rhs>`

Examples:
```
45 * 6
```

##### Supported operand types:
1. `<number> * <number>` -> `number`. Multiplication. Any combination of numbers can be used as operands,
   but the result is lifted to the highest numeric type.


#### / Expression
It's meaning is dependent on the types of the `<lhs>` and `<rhs`.

Syntax: `<lhs> / <rhs>`

Examples:
```
45 / 6
```

##### Supported operand types:
1. `<number> / <number>` -> `number`. Division. Any combination of numbers can be used as operands,
   but the result is lifted to the highest numeric type.


#### % Expression
It's meaning is dependent on the types of the `<lhs>` and `<rhs`.

Syntax: `<lhs> % <rhs>`

Examples:
```
45 % 6
```

##### Supported operand types:
1. `<number> % <number>` -> `number`. Modulo. Any combination of numbers can be used as operands,
   but the result is lifted to the highest numeric type.


#### ^ Expression
It's meaning is dependent on the types of the `<lhs>` and `<rhs`.

Syntax: `<lhs> ^ <rhs>`

Examples:
```
45 ^ 6
```

##### Supported operand types:
1. `<number> ^ <number>` -> `number`. Exponent/power. Any combination of numbers can be used as operands,
   but the result is lifted to the highest numeric type.


#### in Expression
Expresses subset inclusion - i.e, if the `lhs` is a subset of the `rhs`. Note though that the `lhs`
can be a single value, or a set. The individual values must be of the same type.

Syntax: `<lhs> in <rhs>`

Examples:
```
45 in [6, 12, 45] //check if single value is in the set
@resource.[VolumeNumber] in [1223, 2234, 5418] //check if the lhs set is a subset of the rhs set.
```

##### Supported operand types:
All types are supported, except for `boolean`


#### contains Expression
Expresses subset inclusion - i.e, if the `lhs` is a superset of the `rhs`. Note though that the `rhs`
can be a single value, or a set. The individual values must be of the same type.

Syntax: `<lhs> contains <rhs>`

Examples:
```
[6, 12, 45] contains 45 //check if single value is in the set
@resource.[VolumeNumber] contains [1223, 2234, 5418] //check if the rhs set is a subset of the lhs set.
```

##### Supported operand types:
All types are supported, except for `boolean`


#### except Expression
Operates on two sets, expressing the set difference - i.e, All elements of `lhs` that are not in
`rhs`

Syntax: `<lhs> except <rhs>`

Examples:
```
[6, 12, 45] except [45, 82, 0] -> [6, 12]
```

##### Supported operand types:
All types are supported, except for `boolean`


#### exclusion Expression
Operates on two sets, expressing a union of the intersections complement (a mothtfull :p) - i.e,
All elements of `lhs` and `rhs` that are not common among the two sets.

Syntax: `<lhs> exclusion <rhs>`

Examples:
```
[6, 12, 45] except [45, 82, 0] -> [0, 6, 12, 82]
```

##### Supported operand types:
All types are supported, except for `boolean`

#### Relational Expression
Relation expressions test how 2 values compare to one another with respect to scale. The typical
operators are supported:
1. `=`, `!=`: tests equality/equivalence, or non equality/equivalence.
2. `<`, `<=`: tests if a value is less, or equal to another value.
3. `>`, `>=`: tests if a value is greater, or equal to another value.

##### Supported operand types:
All types are supported, except for `boolean`. Type testing is homogenous.


### Operator Precedence

| Type | Category | Operators | Notes |
|------|----------|-----------|-------|
| Unary Operators| N/A | `()`, `present`, `absent`, `not` | Evaluation is left - right, so left-appearing operators will be applied first|
| Binary Operators | Exponent | `^` |
| | Multiplicative | `*`, `/`, `%` |
| | Additive | `+`, `-` |
| | Relational | `<`


### Numeric type precedence
1. `bigdecimal`/`biginteger`
2. `real`
3. `decimal`
4. `integer`
5. `character`*

When a `biginteger` is operated on along with a `real` or `decimal`, the result is lifted to a
`bigdecimal`.

> *__NOTE__: Characters can participate in all numeric-type operations. They are lifted to integers first,
then to whatever type is appropriate for the operation at hand.