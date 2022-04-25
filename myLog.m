function ret=myLog(x,N)
if(nargin==1)
    N=5;
end
B=1:N;%auxiliary vector [1,2,...,N]
c=1:N;%vector for signs in the Taylor series 
c(:)=-1;
a=(x-1)'.^B.*(c.^(B+1));%(x-1) in subsequent powers up until N with correct signs (x-1 because the formula is for x+1)
a=a./B;%dividing by 1:N to get the Taylor series
ret=sum(a,2);%I want to return the sums of each row, as i-th row represents a Taylor series for i-th value from x
ret=ret';%I want my results to be in the same format as results from the built-in fuction log(x)
end

