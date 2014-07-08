@IF "%PROGRAMFILES(x86)%" == "" (
@"%PROGRAMFILES%\Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe" %cd%\run-tests.fsx
) ELSE (
@"%PROGRAMFILES(X86)%\Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe" %cd%\run-tests.fsx
)
