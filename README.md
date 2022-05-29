# fxckMBR
Overwrite MBR with custom message

## MBR
First partition of the your disk. Located in **\\.\PHYSICALDRIVE0** and size of **512** byte. If it's overwrited, you won't able to turn on your PC. However, there are **fix** `(bootrec /rebuildbcd && fixmbr)` command that can revert back your **mbr** to default state. 

## Writing Flat

> **Standard - Flat** overwriting can be done with empty **512** length byte array which will cause this screen;
![](https://i0.wp.com/neosmart.net/wiki/wp-content/uploads/sites/5/2014/06/reboot-select-proper-boot-device.png?w=980&ssl=1)

```csharp
byte[] data = new byte[512];
```

## Writing Custom Message
> To write custom message to this screen, you'll need **single 7-bit ASCII** array which contains **ASCII** of the each **char.**
> You can convert your string to **byte arr** with;
```csharp
byte[] bBytes = Encoding.ASCII.GetBytes("the custom msg to write");
```

# Warning
> MBR overwrite may broke your system. Use at your own risk, recommended using in **vm**.
