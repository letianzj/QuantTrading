## convert character string to POSIXlt date-time object
args(strptime)		
t0 <- strptime("2013-05-20 08:30:30.437", "%Y-%m-%d %H:%M:%OS")	# (str, format, tz = "timezone")
