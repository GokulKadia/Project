@echo off

REM |Version=1.0.0.5|

REM * DELL PROPRIETARY INFORMATION
REM *
REM * This software is confidential.  Dell Inc., or one of its subsidiaries, has
REM * supplied this software to you under the terms of a license agreement,
REM * nondisclosure agreement or both.  You may not copy, disclose, or use this 
REM * software except in accordance with those terms.
REM * 
REM * Copyright 2018-2020 Dell Inc.  All Rights Reserved.
REM *
REM * DELL INC. MAKES NO REPRESENTATIONS OR WARRANTIES
REM * ABOUT THE SUITABILITY OF THE SOFTWARE, EITHER EXPRESS
REM * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
REM * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
REM * PARTICULAR PURPOSE, OR NON-INFRINGEMENT. DELL SHALL
REM * NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE
REM * AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
REM * THIS SOFTWARE OR ITS DERIVATIVES.

md "C:\Program Files\Wyse\WESAppLauncher"
timeout -t 02
xcopy /s /i /q /Y "C:\Temp\CWALauncher.exe" "C:\Program Files\Wyse\WESAppLauncher\CWALauncher.exe*"
timeout -t 02