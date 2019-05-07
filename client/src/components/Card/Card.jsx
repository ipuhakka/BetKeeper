import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Button from 'react-bootstrap/Button';
import './Card.css';

/*
Card component is designed to use fa-awesome icons as card images.
*/
class Card extends Component{

  render(){
    return(
      <div onClick={() => {this.props.onClick()}} className="card">
        <i className={this.props.image}/>
        <h3>{this.props.title}</h3>
        <div>{this.props.text}</div>
        <Button className="cardButton">{"Go to"}</Button>
      </div>
    );
  };
};

Card.propTypes = {
  image: PropTypes.string,
  title: PropTypes.string,
  text: PropTypes.string,
  data: PropTypes.array,
  onClick: PropTypes.func
}

export default Card;
