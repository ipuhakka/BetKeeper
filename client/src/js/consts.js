const ConstVars = {
	URI: process.env.NODE_ENV === 'development'
		? "https://localhost:44324/api/"
		: "https://betkeeper-api.azurewebsites.net/api/",
	DATEPICKER_FORMAT: 'yyyy-MM-dd HH:mm:ss',
	DATETIME_FORMAT: 'YYYY-MM-DD HH:mm:ss'
}

export default ConstVars;