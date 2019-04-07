import React, { Component } from 'react';
import PropTypes from 'prop-types';
import './Tag.css';

class Tag extends Component {
  render(){
    return (
      <div className="tag">
        {this.props.value}
        <i className="fas fa-times-circle img" onClick={this.props.onClick.bind(this, this.props.value)}></i>
      </div>
    );
  }


}

Tag.propTypes = {
  value: PropTypes.string.isRequired,
  onClick: PropTypes.func.isRequired
};

export default Tag;
