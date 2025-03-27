# VisualSploit
This project is based off of a Visual Studio exploit used by North Korean hackers to poison a Visual Studio Project file such that it executes a payload once launched.

I do **NOT** encourage malicious use of this code. This was made for educational purposes only.

Credits: [https://visualstudiomagazine.com/articles/2021/02/01/dprk-attack.aspx](https://visualstudiomagazine.com/articles/2021/02/01/dprk-attack.aspx)

# Features:
- Poison vbproj/csproj file by injecting malicious code
- Poisoned file remote payload via direct download link
- Schtasks persistence
- Startup persistence
- Disable Windows Defender (in progress)
- Fake error message & demonstration

# Usage:
- Build the project
- Execute VisualSploit.exe
- Enter the direct download link to a payload
- Select the vb/csproj file to poison
- Select which AV evasion/persistence options you'd like to enable
- Choose to customize a fake error message
NOTE: Builder logs are just for show, you can disable the task delay in lines 168-218 of Form1.cs :)

# Updates (New):
- N/A

# Demo of Usage:
View Demo Video: N/A

# Contact Me:
Discord: melted3294

Instagram: @m.axx_h

LinkedIn: [Max Harari](https://www.linkedin.com/in/max-harari-b35231359)
