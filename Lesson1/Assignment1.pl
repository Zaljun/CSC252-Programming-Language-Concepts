# 0. Gcd
# gcd using recursion
sub gcd
{
    my $a = $_[0];
    my $b = $_[1];
    if($a == 0) 
    {$b;}
    else
    {gcd($b % $a,$a);}
}
print gcd(319,377),"\n"; 
# 29

# gcd using loop
sub gcd2
{
    my $a = $_[0];
    my $b = $_[1];
    my $save;
    while($b % $a != 0)
    {
        $save = $a;
        $a = $b % $a;
        $b = $save;
    }
    $save = $a;
    $a = $b % $a;
    $b = $save;
    $b;
}
print gcd2(319,377),"\n"; 
# 29

# 1. thereexists
# use for-each loop
sub thereexists
{
    my ($f,@A) = @_;
    my $answer;
    foreach my $x (@A)
    {
        if($f->($x)) {$answer = 1;}
    }
    $answer;
}
print thereexists( sub{$_[0]>0}, (3,5,-2,1,0,4)),"\n"; 
# 1

# 2. doubles
# use for-each loop
sub doubles
{
    my @list;
    foreach my $x (@_)
    {
        push(@list,$x);
        push(@list,$x);
    }
    @list;
}
print doubles(1,2,3),"\n"; 
# 112233

# 3. how-many
# use for each loop
sub howmany
{
    my ($f,@A) = @_;
    my $n = 0;
    foreach my $x (@A)
    {
        if($f->($x)) {$n++;}
    }
    $n;
}
print howmany( sub{$_[0] > 5}, (3,6,2,8,1) ),"\n"; 
# 2