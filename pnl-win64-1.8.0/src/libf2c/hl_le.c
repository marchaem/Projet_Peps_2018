#include "pnl/pnl_f2c.h"

#ifdef KR_headers
extern integer s_cmp();
shortlogical hl_le(a,b,la,lb) char *a, *b; ftnlen la, lb;
#else
extern integer s_cmp(char *, char *, ftnlen, ftnlen);
shortlogical hl_le(char *a, char *b, ftnlen la, ftnlen lb)
#endif
{
return(s_cmp(a,b,la,lb) <= 0);
}
