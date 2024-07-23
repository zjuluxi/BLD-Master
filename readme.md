# BLD-Master

A customizable Rubik's Cube (333 & 444) blindfolded scramble generator.

The difficulty of a 3bf/4bf scramble is sufficiently determined by its conjugacy class.
We first present the following definition:
+ The *orientation* (**ori**) of an edge/corner: for each of the 12 physical edges, we define its characteristic face as UF, UR, UB, UL, DF, DR, DB, DL, FR, FL, BR, BL, respectively. The orientation of an edge is defined as the number of clockwise rotations from the characteristic face of its current position to its physical characteristic face, thus the ori takes values in $\mathbb{Z}_2$. For corners, the characteristic face are UFL, UBL, UBR, UFR, DFL, DBL, DBR, DFR, resp., and the ori takes values in $\mathbb{Z}_3$. For a valid 3bf/4bf state, the ori sum of all edges/corners is 0. The wings/x-centers of 4bf have no ori.
+ The $(j,k)$-cycle: A cycle is defined as a sequence of edges/corners $\{e_1, e_2, \cdots, e_j\}$ such that the physical piece $e_{(i+1)\text{mod }j}$ is at the position of $e_i$ for $i=1,2,\cdots,j$. The cycle is called a $(j,k)$-cycle if it contains $j$ pieces and the ori sum is $k$ (don't forget to take modulo over 2 or 3, for edge/corner resp.). For complicity of representation, we define pieces already on its position as a $(1,ori)$-cycle. A $(1,1)$-cycle of edges is called a *flip*, and a $(1,1)$/$(1,2)$-cycle of corners is called a *twist*.
+ A $(j,0)$-cycle is also called a closed $j$-cycle, and a $(j,k)$-cycle with $k\neq0$ is also called an open $j$-cycle. A closed $(2k+1)$-cycle ($k\geq 1$) can be decomposed into $k$ closed 3-cycles, thus can be solved individually.
+ The *conjugacy class* (**cc**): In group theory, two elements $a,b\in G$ are conjugate if there exists an element $g\in G$ such that $g^{-1}ag=b$, and the maximal set of elements conjugate to each other are called a conjugacy class. In the context of Rubik's Cube, a conjugacy class is completely determined by all its cycles $(j_i, k_i)$. It's quite obvious, as $g$ is known as *set up* in the bf community, which acts on closed 3-cycles $a$ as another closed 3-cycle.
+ The parity of a cycle/conjugacy class: odd-cycles have parity 0 and they can be solved purely by 3-cycles, while even-cycles have parity 1 and they can be solved by 3-cycles plus one 2-cycle. The parity of a conjugacy class is the parity sum of its cycles. For a valid 3bf state, the edge parity is always equal to the corner parity. For 4bf, the parity of wings, corners and x-centers are independent.

For common 3bf/4bf players, they use one buffer to implement 3-cycles, so the cycle containing the buffer should have a special treatment, and we call it the first cycle.
Thus, in the context of this program, the conjugacy class is determined by the first cycle parameters $(perm, ori)$ and an ordered list of other cycle parameters.
The code length is defined as `OtherCycles.Sum(x => x.perm > 1 ? x.perm + 1 : x.ori * 2) + FirstCycle.perm - 1`.
The related classes are called `EdgeCC`, `CornerCC`, `WingCC` and `XCenterCC` respectively.
At present the `XCenterCC` is not implemented and only support scrambled/unscrambled.

All possible classes are stored in the text files in the `Cache` folder, in the format `firstCycleLength, firstCycleOri, secondCycleLength, secondCycleOri, ...`.
As an example, the first line in `ee.txt` (even edge), i.e. `11,1,1,1`, means the cycle containing buffer is a 11-cycle with ori sum being 1, and the remain edge is flipped.
The other cache files are: `oe.txt` (odd edge), `ec.txt` (even corner), `oc.txt` (odd corner), `w.txt` (wing).

This program can generate scrambles with custom constraints for the conjugacy class, where the constraints are defined in `Cube3Class` and `Cube4Class`.
A `Cube3Class` contains a `Predicate<EdgeCC>` for edge conjugacy class constraint, a `Predicate<CornerCC>` for corner conjugacy class constraint and a `Predicate<int>` for parity constraint.
Many examples are shown in `Program.cs`.
An example for generating 10 3bf scrambles with edge code length 10 or 11 and corner code length 6 or 7 without flip or twist is:

```csharp
var cc = new Cube3Class{
    parityLimit = x => x,
    edgeLimit = x => x.CodeLength is >=10 and <= 12 && x.FlipCount == 0,
    cornerLimit = x => x.CodeLength is >=6 and <= 8 && x.TwistCount == 0,
};
cc.Init();

Console.WriteLine($"Probability={cc.probability}"); // Probability=0.8693965505366232

for (int i = 0; i < 10; i++)
{
    var cube = cc.GetCube3();
    System.Console.Write($"{i+1}. ");
    Console.WriteLine(cube.GetScramble());
    // 1. F2 D' R2 D  L2 R2 D2 R2 D' R2 U' B  U' L  D  B2 R' B' R  D2 U  
    // 2. L  U  B' R  U  D  B' R  U2 F' R2 F2 U2 D2 F  R2 U2 F2 D2 F' U  
    // 3. F' U  L' D2 L' U2 L' R2 B2 D2 R  D2 R2 U' B2 R' F' U2 R' B  U  
    // 4. D2 B2 R2 B2 U' L2 U' L2 B2 U  F2 U' R' F' L' B' R' B2 U2 B  D  
    // 5. L2 B2 R' U2 R' U2 F2 L' B2 R2 B2 U2 B  D  L  F' L' B  R' U2 F2 
    // 6. F  R  L2 F  D' F  D' F' D2 B' L2 F  U2 F  R2 B' L2 R  B2       
    // 7. R2 B2 R2 D2 F2 D' U2 B2 R2 D  L2 D' B  R  D' R2 U2 F  D'       
    // 8. L' F2 D2 R2 F' D2 U2 R2 U2 F' D2 U2 R2 D' F' L2 D' R' D2 L' U2 
    // 9. U  F2 U2 D' L  B  L' B2 U2 D2 R2 U2 R' D2 R' U2 F2 L' D' F     
    // 10. B2 R2 F2 L2 D2 F' L2 F2 D2 B' R2 D' F2 D  R' U2 R' D  U
}
```

A `Cube4Class` contains a `Predicate<WingCC>`, a `Predicate<CornerCC>` and a `bool` for whether to scramble x-centers.
An example for generating a 4bf scramble with wing code length 24 without scrambling corners or x-centers is:

```csharp
var cc = new Cube4Class{
    wingLimit = x => x.CodeLength == 24,
    cornerLimit = x => x.CodeLength == 0,
    scrambleXCenter = false
};
cc.Init();
Console.WriteLine($"Probability={cc.probability}"); // Probability=0 ...todo

var cube = cc.GetCube4();
Console.WriteLine(cube.GetScramble());
// R'  U'  B2  D'  B2  R2  D'  R2  U'  R2  U   B'  L'  F2  R'  B'  U'  D'  F   L2  Rw2 Fw2 D2  B'  Uw2 D   F   D2  U   Fw2 U2  F   L   Uw2 U2  F'  Uw2 x2
```

To use the program well, it's best to customize your need in `Program.cs` by yourself.

## Some statistic results

### Basic info about conjugacy class

| Name | Value |
| --- | --- |
| `EdgeCC.evenList.Count` | 988 |
| `EdgeCC.oddList.Count` | 979 |
| `CornerCC.evenList.Count` | 428 |
| `CornerCC.oddList.Count` | 416 |
| `WingCC.list.Count` | 5763 |

### The distribution of cycle code length and flip/twist count

*Notation: 10+1 means cycle code length 10 with 1 flip/twist.*
*Order by: probability*

#### Edges:

| Code Length | Count | Probability |
| --- | --- | --- |
| 12+0 | 215574955264 | 0.219751 |
| 11+0 | 171162393600 | 0.174478 |
| 13+0 | 121975541248 | 0.124339 |
| 11+1 | 100186352640 | 0.102127 |
| 10+1 | 82582917120 | 0.0841828 |
| 10+0 | 61002965760 | 0.0621848 |
| 12+1 | 52445828864 | 0.0534619 |
| 14+0 | 31151605760 | 0.0317551 |
| 9+1 | 29774997120 | 0.0303518 | 
| 10+2 | 23045045760 | 0.0234915 |
| 9+2 | 19837635840 | 0.0202219 | 
| 9+0 | 13284416640 | 0.0135418 | 
| 13+1 | 11560299520 | 0.0117843 |
| 11+2 | 10980541440 | 0.0111933 |
| 8+2 | 7244497920 | 0.00738484 | 
| 8+1 | 6516026880 | 0.00664226 | 
| 9+3 | 3488390400 | 0.00355597 | 
| 15+0 | 3229219840 | 0.00329178 |
| 8+3 | 3160058880 | 0.00322128 | 
| 8+0 | 2052272640 | 0.00209203 | 
| 12+2 | 1999486720 | 0.00203822 |
| 7+2 | 1593789120 | 0.00162467 | 
| 10+3 | 1478400000 | 0.00150704 |
| 7+3 | 1170597120 | 0.00119327 | 
| 7+1 | 1009008000 | 0.00102856 | 
| 14+1 | 887040000 | 0.000904225 |
| 8+4 | 389368320 | 0.000396912 | 
| 7+4 | 374996160 | 0.000382261 | 
| 6+3 | 258978720 | 0.000263996 | 
| 6+2 | 247373280 | 0.000252166 | 
| 7+0 | 244276032 | 0.000249008 |
| 11+3 | 206976000 | 0.000210986 |
| 9+4 | 141778560 | 0.000144525 |
| 6+4 | 141150240 | 0.000143885 |
| 6+1 | 120216096 | 0.000122545 |
| 13+2 | 97574400 | 9.94647E-05 |
| 16+0 | 92252160 | 9.40394E-05 |
| 5+3 | 40286400 | 4.10669E-05 |
| 6+5 | 35282016 | 3.59655E-05 |
| 7+5 | 33973632 | 3.46318E-05 |
| 5+4 | 31416000 | 3.20246E-05 |
| 5+2 | 29494080 | 3.00655E-05 |
| 6+0 | 23581536 | 2.40384E-05 |
| 5+5 | 13527360 | 1.37894E-05 |
| 10+4 | 13305600 | 1.35634E-05 |
| 5+1 | 11605440 | 1.18303E-05 |
| 15+1 | 10644480 | 1.08507E-05 |
| 8+5 | 10053120 | 1.02479E-05 |
| 4+4 | 4897200 | 4.99207E-06 |
| 4+3 | 4804800 | 4.89788E-06 |
| 12+3 | 4435200 | 4.52112E-06 |
| 4+5 | 3030720 | 3.08943E-06 |
| 4+2 | 2845920 | 2.90105E-06 |
| 5+6 | 2735040 | 2.78803E-06 |
| 6+6 | 2387616 | 2.43387E-06 |
| 5+0 | 1911360 | 1.94839E-06 |
| 4+6 | 1071840 | 1.0926E-06 |
| 4+1 | 939840 | 9.58047E-07 |
| 3+4 | 582120 | 5.93397E-07 |
| 7+6 | 517440 | 5.27464E-07 |
| 3+5 | 471240 | 4.80369E-07 |
| 3+3 | 462000 | 4.7095E-07 |
| 9+5 | 443520 | 4.52112E-07 |
| 3+6 | 240240 | 2.44894E-07 |
| 3+2 | 229680 | 2.3413E-07 |
| 4+7 | 179520 | 1.82998E-07 |
| 5+7 | 137280 | 1.3994E-07 |
| 4+0 | 133320 | 1.35903E-07 |
| 3+7 | 71280 | 7.26609E-08 |
| 3+1 | 65340 | 6.66058E-08 |
| 2+4 | 55440 | 5.6514E-08 |
| 2+5 | 55440 | 5.6514E-08 |
| 2+3 | 36960 | 3.7676E-08 |
| 2+6 | 36960 | 3.7676E-08 |
| 2+2 | 15840 | 1.61469E-08 |
| 2+7 | 15840 | 1.61469E-08 |
| 6+7 | 15840 | 1.61469E-08 |
| 3+8 | 9900 | 1.00918E-08 |
| 3+0 | 8140 | 8.2977E-09 |
| 4+8 | 6600 | 6.72786E-09 |
| 1+5 | 5544 | 5.6514E-09 |
| 1+4 | 4620 | 4.7095E-09 |
| 1+6 | 4620 | 4.7095E-09 |
| 2+1 | 3960 | 4.03672E-09 |
| 2+8 | 3960 | 4.03672E-09 |
| 1+3 | 2640 | 2.69114E-09 |
| 1+7 | 2640 | 2.69114E-09 |
| 1+2 | 990 | 1.00918E-09 |
| 1+8 | 990 | 1.00918E-09 |
| 0+5 | 462 | 4.7095E-10 |
| 0+6 | 462 | 4.7095E-10 |
| 2+0 | 440 | 4.48524E-10 |
| 2+9 | 440 | 4.48524E-10 |
| 0+4 | 330 | 3.36393E-10 |
| 0+7 | 330 | 3.36393E-10 |
| 1+1 | 220 | 2.24262E-10 |
| 1+9 | 220 | 2.24262E-10 |
| 3+9 | 220 | 2.24262E-10 |
| 0+3 | 165 | 1.68197E-10 |
| 0+8 | 165 | 1.68197E-10 |
| 0+2 | 55 | 5.60655E-11 |
| 0+9 | 55 | 5.60655E-11 |
| 1+0 | 22 | 2.24262E-11 |
| 1+10 | 22 | 2.24262E-11 |
| 0+1 | 11 | 1.12131E-11 |
| 0+10 | 11 | 1.12131E-11 |
| 0+0 | 1 | 1.01937E-12 |
| 0+11 | 1 | 1.01937E-12 |

#### Corners

| Code Length | Count | Probability |
| --- | --- | --- |
| 8+0 | 19292256 | 0.218783 |
| 7+0 | 16528617 | 0.187442 |
| 7+1 | 11369484 | 0.128935 |
| 6+1 | 10542798 | 0.11956 |
| 9+0 | 7302393 | 0.0828125 |
| 6+0 | 4468527 | 0.0506752 |
| 8+1 | 3470040 | 0.0393519 |
| 5+2 | 3333960 | 0.0378086 |
| 6+2 | 3245508 | 0.0368056 |
| 5+1 | 2891700 | 0.0327932 |
| 4+2 | 929880 | 0.0105453 |
| 7+2 | 714420 | 0.00810185 |
| 4+3 | 695520 | 0.00788752 |
| 10+0 | 688905 | 0.0078125 |
| 5+0 | 686070 | 0.00778035 |
| 5+3 | 589680 | 0.00668724 |
| 4+1 | 446040 | 0.0050583 |
| 3+3 | 196560 | 0.00222908 |
| 9+1 | 153090 | 0.00173611 |
| 3+2 | 143640 | 0.00162894 |
| 3+4 | 105840 | 0.00120027 |
| 4+4 | 75600 | 0.000857339 |
| 4+0 | 72765 | 0.000825189 |
| 6+3 | 68040 | 0.000771605 |
| 3+1 | 47250 | 0.000535837 |
| 2+3 | 30240 | 0.000342936 |
| 2+4 | 30240 | 0.000342936 |
| 2+2 | 15120 | 0.000171468 |
| 2+5 | 12096 | 0.000137174 |
| 3+5 | 6048 | 6.85871E-05 |
| 3+0 | 5859 | 6.64438E-05 |
| 1+4 | 5040 | 5.71559E-05 |
| 1+5 | 4032 | 4.57247E-05 |
| 2+1 | 3780 | 4.28669E-05 |
| 1+3 | 3360 | 3.81039E-05 |
| 1+6 | 1344 | 1.52416E-05 |
| 1+2 | 1260 | 1.4289E-05 |
| 0+5 | 672 | 7.62079E-06 |
| 0+4 | 560 | 6.35066E-06 |
| 0+6 | 448 | 5.08053E-06 |
| 2+0 | 378 | 4.28669E-06 |
| 0+3 | 280 | 3.17533E-06 |
| 1+1 | 252 | 2.8578E-06 |
| 0+7 | 128 | 1.45158E-06 |
| 0+2 | 84 | 9.52599E-07 |
| 1+0 | 21 | 2.3815E-07 |
| 0+1 | 14 | 1.58766E-07 |
| 0+0 | 1 | 1.13405E-08 |

### The distribution of code length

*Code Length = Cycle Code Length + 2 * Flip/Twist Count*

#### Edge

| Alg Count | Count | Probability |
| --- | --- | --- |
| 0 | 1 | 1.019373E-12 |
| 1 | 22 | 2.24262E-11 |
| 2 | 451 | 4.597372E-10 |
| 3 | 8360 | 8.521957E-9 |
| 4 | 137335 | 1.399956E-7 |
| 5 | 1977690 | 2.016004E-6 |
| 6 | 24537381 | 2.501274E-5 |
| 7 | 256113792 | 2.610755E-4 |
| 8 | 2175371946 | 2.217515E-3 |
| 9 | 14323385340 | 1.460087E-2 |
| 10 | 67771226622 | 6.908415E-2 |
| 11 | 202572053904 | 2.064965E-1 |
| 12 | 305666302126 | 3.115879E-1 |
| 13 | 243202018708 | 2.479135E-1 |
| 14 | 109946757514 | 1.120767E-1 |
| 15 | 29647217600 | 3.022157E-2 |
| 16 | 4882917061 | 4.977513E-3 |
| 17 | 493754382 | 5.033198E-4 |
| 18 | 30365071 | 3.095333E-5 |
| 19 | 1108360 | 1.129832E-6 |
| 20 | 22891 | 2.333447E-8 |
| 21 | 242 | 2.466882E-10 |
| 22 | 1 | 1.019373E-12 |

#### Corner

| Code Length | Count | Probability |
| --- | --- | --- |
| 0 | 1 | 1.019373E-12 |
| 1 | 21 | 2.140683E-11 |     
| 2 | 392 | 3.995942E-10 |    
| 3 | 6111 | 6.229388E-9 |    
| 4 | 76629 | 7.811353E-8 |   
| 5 | 734580 | 7.488109E-7 |  
| 6 | 4929967 | 5.025475E-6 | 
| 7 | 19567317 | 1.994639E-5 |
| 8 | 30795734 | 3.139234E-5 |
| 9 | 22207437 | 2.263766E-5 |
| 10 | 8130885 | 8.288404E-6 |
| 11 | 1567062 | 1.597421E-6 |
| 12 | 156184 | 1.592097E-7 | 
| 13 | 7392 | 7.535204E-9 |   
| 14 | 128 | 1.304797E-10 |

#### Wings

| Code Length | Count | Probability |
| --- | --- | --- |
| 0 | 1 | 1.611738E-24 |
| 1 | 23 | 3.706996E-23 |      
| 2 | 506 | 8.155392E-22 |     
| 3 | 10879 | 1.753409E-20 |   
| 4 | 221375 | 3.567984E-19 |  
| 5 | 4268110 | 6.879073E-18 | 
| 6 | 77890351 | 1.255388E-16 |
| 7 | 1342157663 | 2.163206E-15 |
| 8 | 21771902856 | 3.509059E-14 |
| 9 | 331314529029 | 5.339921E-13 |
| 10 | 4710572853855 | 7.592207E-12 |
| 11 | 62281939724400 | 1.003821E-10 |
| 12 | 761641280892226 | 1.227566E-9 |
| 13 | 8560379076084878 | 1.379708E-8 |
| 14 | 87772465770520916 | 1.414662E-7 |
| 15 | 813763800010017934 | 1.311574E-6 |
| 16 | 6749402976686974670 | 1.087827E-5 |
| 17 | 49425347848460992540 | 7.966069E-5 |
| 18 | 314337425953333777126 | 5.066294E-4 |
| 19 | 1699899859773132158438 | 2.739792E-3 |
| 20 | 7601948340237250714176 | 1.225235E-2 |
| 21 | 27067509119021061290754 | 4.362572E-2 |
| 22 | 72780176609174913594870 | 1.173025E-1 |
| 23 | 137244486165479904860880 | 2.212021E-1 |
| 24 | 165918222511005044646061 | 2.674166E-1 |
| 25 | 125191973150261438747723 | 2.017766E-1 |
| 26 | 59836132878940606224146 | 9.644014E-2 |
| 27 | 18479874254757697043779 | 2.978471E-2 |
| 28 | 3725887117117167465155 | 6.005152E-3 |
| 29 | 488170599527975742070 | 7.868029E-4 |
| 30 | 40601722742510564635 | 6.543932E-5 |
| 31 | 2039271664676921675 | 3.286771E-6 |
| 32 | 56316997446910200 | 9.076822E-8 |
| 33 | 711491685129225 | 1.146738E-9 |
| 34 | 2635284526875 | 4.247387E-12 |

### Alg Count

#### Edge: 1 cycle code or 1 flip as 0.5 alg

| Alg Count | Probability  |
| ---       | ---          |
| 0	        | 1.019373e-12 |
| 0.5       | 2.242620e-11 |
| 1	        | 5.158027e-10 |
| 1.5       | 9.531137e-09 |
| 2	        | 1.565910e-07 |
| 2.5       | 2.256525e-06 |
| 3	        | 2.799261e-05 |
| 3.5       | 0.0002919788 |
| 4	        | 0.002476726  |
| 4.5       | 0.01626882   |
| 5	        | 0.07662396   |
| 5.5       | 0.2266443    |
| 6	        | 0.3310823    |
| 6.5       | 0.2413949    |
| 7	        | 0.08878605   |
| 7.5       | 0.01538694   |
| 8	        | 0.001002785  |
| 8.5       | 1.085069e-05 |

Expectation = 6.014471

#### Corner: 1-3 twists as 1 alg, 4-6 twists as 2 algs, 7 as 3

| Alg Count | Probability  |
| ---       | ---          |
| 0	        | 1.134046e-08 |
| 0.5       | 2.381497e-07 |
| 1	        | 6.191891e-06 |
| 1.5       | 9.311652e-05 |
| 2	        | 0.001143357  |
| 2.5       | 0.01063910   |
| 3	        | 0.06899270   |
| 3.5       | 0.2626665    |
| 4	        | 0.3821147    |
| 4.5       | 0.2248650    |
| 5	        | 0.04774306   |
| 5.5       | 0.001736111  |

Expectation = 3.943956

#### A full 333 cube

| Alg Count | Probability  |
| ---       | ---          |
| 0         | 2.312032e-20 |
| 1         | 3.500416e-17 |
| 2         | 2.098640e-14 |
| 3         | 7.221221e-12 |
| 4         | 1.606755e-09 |
| 5         | 2.335786e-07 |
| 6         | 2.114250e-05 |
| 7         | 0.001085375  |
| 8         | 0.0267720    |
| 9         | 0.2362029    |
| 10        | 0.5017181    |
| 11        | 0.2170380    |
| 12        | 0.0170080    |
| 13        | 0.0001540588 |
| 14        | 3.767602e-08 |

Expectation = 9.958427

### Number of Float closed 3-cycle

#### Edge

| Count | Probability  |
| ---   | ---          |
| 0     | 0.881752     |
| 1     | 0.111690     |
| 2     | 0.006366     |
| 3     | 0.000193     |

Expectation = 0.125000

#### Corner

| Count | Probability  |
| ---   | ---          |
| 0     | 0.932099     |
| 1     | 0.066358     |
| 2     | 0.001543     |

Expectation = 0.069444

### Number of other cycles

#### Edge

| Count | Probability  |
| ---   | ---          |
| 0     | 0.226523     |
| 1     | 0.435843     |
| 2     | 0.267955     |
| 3     | 0.064040     |
| 4     | 0.005523     |
| 5     | 0.000116     |

Expectation = 1.186544

#### Corner

| Count | Probability  |
| ---   | ---          |
| 0     | 0.339782     |
| 1     | 0.487996     |
| 2     | 0.161806     |
| 3     | 0.010417     |

Expectation = 0.842857

### Number of flips/twists

#### Edge

| Count | Probability  |
| ---   | ---          |
| 0     | 0.631803     |
| 1     | 0.290629     |
| 2     | 0.066339     |
| 3     | 0.010004     |
| 4     | 0.001119     |
| 5     | 9.87192E-05  |
| 6     | 7.12972E-06  |
| 7     | 4.30920E-07  |
| 8     | 2.20337E-08  |
| 9     | 9.53114E-10  |
| 10    | 3.36393E-11  |
| 11    | 1.01937E-12  |

Expectation = 0.458333

#### Corner

| Count | Probability  |
| ---   | ---          |
| 0     | 0.556202     |
| 1     | 0.328017     |
| 2     | 0.095077     |
| 3     | 0.017960     |
| 4     | 0.002464     |
| 5     | 0.000259     |
| 6     | 2.03221E-05  |
| 7     | 1.45158E-06  |

Expectation = 0.583333

### The distribution of the order of Rubik's cube group elements

Given any move sequence, we will eventually return to the initial state if we repeat the sequence enough times. The number of times we need to repeat the sequence is the order of the group element (i.e. the target state by executing the sequence onto a solved cube). The following table shows the distribution of the order of the group elements.

| Order | Count               |
| ---   | ---                 |
| 1     | 1                   |
| 2     | 170911549183        |
| 3     | 33894540622394      |
| 4     | 4346957030144256    |
| 5     | 133528172514624     |
| 6     | 140621059298755526  |
| 7     | 153245517148800     |
| 8     | 294998638981939200  |
| 9     | 55333752398428896   |
| 10    | 34178553690432192   |
| 11    | 44590694400         |
| 12    | 2330232827455554048 |
| 14    | 23298374383021440   |
| 15    | 14385471333209856   |
| 16    | 150731886270873600  |
| 18    | 1371824848124089632 |
| 20    | 151839445189791744  |
| 21    | 39337151559333120   |
| 22    | 927085127270400     |
| 24    | 3293932519796244480 |
| 28    | 97419760907673600   |
| 30    | 1373347158867028224 |
| 33    | 15874019662233600   |
| 35    | 65526218912563200   |
| 36    | 3768152294808760320 |
| 40    | 835897246403788800  |
| 42    | 737199776831097600  |
| 44    | 100120377950208000  |
| 45    | 197329441659727104  |
| 48    | 911497647410380800  |
| 55    | 4854321355161600    |
| 56    | 671205306846412800  |
| 60    | 4199961633799421952 |
| 63    | 264371433705308160  |
| 66    | 404051175250329600  |
| 70    | 210461722916290560  |
| 72    | 1981453794190295040 |
| 77    | 187238109413376000  |
| 80    | 13349383726694400   |
| 84    | 1697725818678067200 |
| 90    | 1764876446897050368 |
| 99    | 104367909135974400  |
| 105   | 232824419423354880  |
| 110   | 4854321355161600    |
| 112   | 128726200221696000  |
| 120   | 1947044011463147520 |
| 126   | 854783686296207360  |
| 132   | 637129677864960000  |
| 140   | 223125413717606400  |
| 144   | 714192029378150400  |
| 154   | 187238109413376000  |
| 165   | 213590139627110400  |
| 168   | 1050269239266508800 |
| 180   | 2320395168471367680 |
| 198   | 759701292082790400  |
| 210   | 1053174509332070400 |
| 231   | 374476218826752000  |
| 240   | 407156203664179200  |
| 252   | 689877080447385600  |
| 280   | 68653973451571200   |
| 315   | 99309879652515840   |
| 330   | 213590139627110400  |
| 336   | 257452400443392000  |
| 360   | 571019888909352960  |
| 420   | 961155628321996800  |
| 462   | 374476218826752000  |
| 495   | 174755568785817600  |
| 504   | 238381852262400000  |
| 630   | 395380140162416640  |
| 720   | 120144453540249600  |
| 840   | 240288907080499200  |
| 990   | 174755568785817600  |
| 1260  | 51490480088678400   |

### Some other stats that is not covered by this program

#### Number of x-centers or t-centers out of position in 5bf

| Number | Count            | Probability  | 
| ---    | ---              | ---          |
| 0      | 1                | 3.0800784636748E-16 |
| 1      | 0                | 0 |
| 2      | 240              | 7.39218831281952E-14 |
| 3      | 2560             | 7.88500086700748E-13 |
| 4      | 46620            | 1.43593257976519E-11 |
| 5      | 581376           | 1.7906836968974E-10 |
| 6      | 6629680          | 2.04199345890555E-09 |
| 7      | 64154880         | 1.97602064227641E-08 |
| 8      | 539147295        | 1.66061597207802E-07 |
| 9      | 3927917440       | 1.20982939140366E-06 |
| 10     | 24886550016      | 7.66525267394473E-06 |
| 11     | 137209752960     | 4.22616805098235E-05 |
| 12     | 658128223120     | 0.000202708656636847 |
| 13     | 2741884721280    | 0.00084452200798935 |
| 14     | 9892350759360    | 0.00304692165290218 |
| 15     | 30763384990336   | 0.00947536395784705 |
| 16     | 81910672955415   | 0.0252291299715083 |
| 17     | 185000621132160  | 0.0569816428915627 |
| 18     | 349913277503440  | 0.107776035019221 |
| 19     | 544438016772480  | 0.167691181026673 |
| 20     | 679274481651396  | 0.209221870185833 |
| 21     | 654044867816320  | 0.201450951163808 |
| 22     | 456702275019600  | 0.140667884159915 |
| 23     | 206032439164800  | 0.0634596078689888 |
| 24     | 45131501617225   | 0.0139008566164519 |

Sum = 3246670537110000 = $24!/24^6$

Mean = 20.0

### Probability of edge buffer position in full parity (will explain)

No constraints on the parity:

| Position | Count        | Probability         | Cumulative Probability |
| ---      | ---          | ---                 | ---                    |
| #1       | 490497638400 | 0.5                 | 0.5                    |
| #2       | 165357158400 | 0.168560606060606   | 0.668560606060606      |
| #3       | 82864373760  | 0.084469696969697   | 0.753030303030303      |
| #4       | 50180014080  | 0.0511521464646465  | 0.80418244949495       |
| #5       | 33901056000  | 0.0345578177609428  | 0.838740267255892      |
| #6       | 24637063680  | 0.0251143550459957  | 0.863854622301888      |
| #7       | 18881856000  | 0.0192476523042929  | 0.883102274606181      |
| #8       | 15078452544  | 0.0153705658942475  | 0.898472840500428      |
| #9       | 12448644384  | 0.0126898107242752  | 0.911162651224704      |
| #10      | 10568158788  | 0.0107728946692519  | 0.921935545893956      |
| #11      | 9189703292   | 0.00936773449305154 | 0.931303280387007      |
| Else     | 67391157472  | 0.0686967196129929  | 1                      |

Sum = 980995276800 = $2^{11}\times 12!$

Constrained on odd parity:

| Position | Count        | Probability         | Cumulative Probability |
| ---      | ---          | ---                 | ---                    |
| #1       | 241532928000 | 0.492424242424242   | 0.492424242424242      |
| #2       | 79705866240  | 0.1625              | 0.654924242424242      |
| #3       | 39109754880  | 0.0797348484848485  | 0.734659090909091      |
| #4       | 23335280640  | 0.0475747053872054  | 0.782233796296296      |
| #5       | 15690101760  | 0.0319881290584416  | 0.814221925354738      |
| #6       | 11487156480  | 0.0234193920229076  | 0.837641317377645      |
| #7       | 8980616064   | 0.0183091932782688  | 0.855950510655914      |
| #8       | 7398461472   | 0.0150835822495165  | 0.871034092905431      |
| #9       | 6357359112   | 0.0129610391861165  | 0.883995132091547      |
| #10      | 5650322210   | 0.0115195706720043  | 0.895514702763552      |
| #11      | 5158302131   | 0.0105164668026259  | 0.906031169566177      |
| Else     | 46091489411  | 0.0939688304338225  | 1                      |

Sum = 490497638400 = $2^{10}\times 12!$

## Acknowledgements

+ Thank github copilot, without which a lazy boy like me may never write this readme.
+ Many thanks to [Chen Shuang](https://github.com/cs0x7f/min2phase) for the [min2phase](https://github.com/cs0x7f/min2phase) algorithm for solving 333 cube and [TPR-4x4x4-Solver](https://github.com/cs0x7f/TPR-4x4x4-Solver) for solving 444 cube, which I brutally modified to C# in this project.

## License GPL-3.0

```
Copyright (C) 2019-2024 Xi Lu

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
```
