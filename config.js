var config = {
  db_path: 'database/data/data.sqlite3'
};

module.exports = {
    setConfig: function(newConfig){
    if (newConfig.db_path !== undefined){
      config.db_path = newConfig.db_path;
    }
  },

  getConfig: function(){
    return config;
  }
};
