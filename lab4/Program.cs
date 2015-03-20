using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace lab4_new
{
    class Program
    {
        string s;
        int n;
        int sz;
        state ptr;

        class node 
        {
	        int l, r, par = -1, link;
	        map<char,int> next;

            public node(int l, int r, int par)
            {
                this.l = l;
                this.r = r;
                this.par = par;
                this.link = -1;
            }

            int len()  {  return r - l;  }

	        int get (char c) 
            {
		        if (!next.count(c))  
                    next[c] = -1;

		        return next[c];
	        }
        }

        node t[MAXN];
 
        class state 
        {
	        public int v, pos;

            public state(int v, int pos)
            {
                this.v = v;
                this.pos = pos;
            }
        }




        static void Main(string[] args)
        {
            ptr = new state(0,0);
        }

        state go (state st, int l, int r) 
        {
	        while (l < r)
		        if (st.pos == t[st.v].len()) 
                {
			        st = new state (t[st.v].get( s[l] ), 0);

			        if (st.v == -1) 
                        return st;
		        }
		        else 
                {
			        if (s[ t[st.v].l + st.pos ] != s[l])
				        return new state (-1, -1);

			        if (r-l < t[st.v].len() - st.pos)
				        return new state (st.v, st.pos + r-l);

			        l += t[st.v].len() - st.pos;
			        st.pos = t[st.v].len();
		        }

	        return st;
        }
 
        int split (state st) 
        {
	        if (st.pos == t[st.v].len())
		        return st.v;

	        if (st.pos == 0)
		        return t[st.v].par;

	        node v = t[st.v];
	        int id = sz++;

	        t[id] = new node (v.l, v.l+st.pos, v.par);
	        t[v.par].get( s[v.l] ) = id;
	        t[id].get( s[v.l+st.pos] ) = st.v;
	        t[st.v].par = id;
	        t[st.v].l += st.pos;

	        return id;
        }
 
        int get_link (int v) 
        {
	        if (t[v].link != -1)  return t[v].link;
	        if (t[v].par == -1)  return 0;

	        int to = get_link (t[v].par);
	        return t[v].link = split(go(new state(to,t[to].len()), t[v].l + (t[v].par==0), t[v].r));
        }
 
        void tree_extend (int pos) 
        {
	        for(;;) 
            {
		        state nptr = go(ptr, pos, pos+1);
		        if (nptr.v != -1) 
                {
			        ptr = nptr;
			        return;
		        }
 
		        int mid = split (ptr);
		        int leaf = sz++;
		        t[leaf] = new node(pos, n, mid);
		        t[mid].get(s[pos]) = leaf;
 
		        ptr.v = get_link(mid);
		        ptr.pos = t[ptr.v].len();

		        if (!mid)  
                    break;
	        }
        }
 
        void build_tree() 
        {
	        sz = 1;
	        for (int i=0; i<n; ++i)
		        tree_extend (i);
        }
    }
}
