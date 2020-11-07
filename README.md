# vcdestroy
Destroy a VeraCrypt container

I was experimenting with some large VeraCrypt contianers.  They were nearly 2TB.
After my testing it was time to delete these huge containers and recover the storage
space.  I've gotten into the habit of using a delete-beyond-recovery utility for
all file deletions.  It really wasn't necessary with the encrypted containers.  The
delete-beyond-recovery utility makes multiple passes over the file writing random
data patterns to make sure every byte is overwritten.  I knew it would take a long
time to overwrite a 2TB file, but I did it anyway just before I stopped working one
evening.

When I got to my desk the next morning I was suprised to see that the deletion was 
only about 25% complete.  I'm thinking "wow" its going to take 2 days for this to
complete.  I killed it and deleted the file.  

In this case it was just test data, but I got to thinking; what if I had a real need
to absolultely destroy a huge VeraCrypt container?  My mind started churning on how
this could happen.  Perhaps I had created a large volume to store some really 
senstive customer data.  And the super strong password got compromised somehow.  It
would really want to make sure that file was destroyed and not recoverable.

The way a VeraCrypt volume is structured is there is a header section that contains 
salt and encrypted data about the rest of the volume, including encryption keys. If 
the header is destroyed, then the rest of the volume might as well be random data 
because there is no way the contents can be recovered.  So I only need to scramble
the header area of the file and then it won't matter if someone has the passwords,
keyfiles, or whatever else would be required to unlock the data.  Wait a sec!  
VeraCrypt volumes have a backup header after the data area just in case the volume
header gets corrupted.  No problem, lets scramble that backup header too.  Hold on, 
one more thing; if you are using VeraCrypts hidden volume feature, it has its own 
header following the outer volume header, and there is also a backup of the hidden
volume header.  Lets scramble those too.

Done, vcdestroy overwrites all of the headers, including backup headers with random
data.  And just for fun it does it more than a few passes.  Those headers are just 
gone for good!  

One more concern.  It is possible to backup the volume headers to an external file.
That means you should really "delete-beyond-recovery" any backups of the headers to
make sure those can't restored to the volume and make it recoverable again.  Vcdestroy
goes one step further and write random data in strategic locations outside of the
volume headers to make sure that even if the headers were previously backed up and
then restored, the volumes will still not mount and even if they did, they would be
corrupt.  

So you want to blow away a VeraCrypt (or TrueCrypt) volume, just use vcdestroy on
it and it will be securely wiped and deleted, much faster than doing a complete 
overwrite.
