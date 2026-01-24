\ KERNEL WORDS
: [compile] ' compile, ; immediate
: postpone ' litn [ ' compile, litn ] compile, ; immediate
: >r? r> r> rot >r >r >r ;  ( a R: b -- R: a b )
: r>? r> r> r> nrot >r >r ; ( R: a b -- a R: b )
: dump ( a n -- ) >r begin nl> ':' emit dup .x spc>
  4 >r begin dup c@ dup .x >r? 1+ dup c@ dup .x >r? 1+ spc> next
  8 >r begin r>? next 8 >r begin dup $20 < if drop '.' then emit
  next next drop ;

\ BOOT MESSAGE
: memused here @ $500 - ; : memfree $7bff memused - ;
: memstat \ prints current dictionary usage
  memused 1000 / . ." kB used " memfree 1000 / . ." kB free" ;
here @ ," Welcome to TANUKI OS" 20 type nl> memstat

\ BLOCK SUBSYSTEM
here @ 1024 allot value blk( \ start address of block buffer
blk( 1024 + value blk) \ end address of block buffer
-1 value blk? \ currently active block
\ ( n -- addr ) return the address of block n
: >blk 10 lshift $7e00 + ;
\ ( n -- ) interpret forth code from block n
: load >blk blk ! 16 line ! ;
\ ( n1 n2 -- ) load blocks between n1 and n2, inclusive
: thru over - 1+ 4 lshift line ! >blk blk ! ;
\ ( n -- ) display n right aligned by 2 spaces
: .2 nl> dup 10 < if spc> then . spc> ;
\ ( addr -- ) print the contents as a block at addr
: display 16 >r begin 16 r@ - .2 dup emitln 64 + next drop ;
\ ( n -- ) print the contents of block n
: list >blk display ; 
\ ( -- ) display current buffer content
: blk. blk( display ;
\ ( n -- ) read block n into buffer and make n active
: blk@ dup to blk? >blk blk( 1024 move ;
\ ( s d -- ) copy contents of block s to block d
: copy >blk >r >blk r> 1024 move ;
\ ( n -- ) convert block number to sector number in hard drive
: >sector 2* 18 + ;
\ ( -- ) write current block to disk
: flush blk( blk? >sector write
  if ." success!" else ." failed!" exit then
  blk( blk? >blk 1024 move ;
\ ( -- ) emties current block
: wipe blk( 1024 0 fill ;


\ BLOCK EDITOR
\ word for drawing text at coordinate
\ ctrl-w for save
: rand 25173 * 13849 + ;
: a 1 begin dup 7 mod 191 + emit rand 0 10000 usleep again ;
