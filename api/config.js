var config = {
  db_path: 'default filename'
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
