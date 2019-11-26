import React, { Component } from 'react';
import PropTypes from 'prop-types';
import './Spinner.css';

// Loading animation
class Spinner extends Component {

  render(){
    if (!this.props.active)
    {
      return null;
    }

    return (
      <div>
        <div>{this.props.text}</div>
        <div className="spinner"/>
      </div>
    );
  }
};

Spinner.propTypes = {
  active: PropTypes.bool.isRequired,
  text: PropTypes.string
};

export default Spinner;
