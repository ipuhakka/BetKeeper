import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Card from 'react-bootstrap/Card';
import Button from './Button';
import './CardMenu.css';

class CardMenu extends Component
{


    render()
    {
        const { cards, onClick } = this.props;

        const cardItems = cards.map((card, i) => {
            const { image, title, text, navigateTo } = card;
            return <Card key={`card-${i}`}>
            <div>
              <i className={image}/>
            </div>
            <Card.Body className="cardBody">
              <Card.Title>{title}</Card.Title>
              <Card.Text>{text}</Card.Text>
              <Button
                onClick={onClick} 
                className="cardButton" 
                navigateTo={navigateTo} 
                text={'Go to'}
                buttonStyle='primary'
                buttonType='Navigation'
                displayType='Text'></Button>
            </Card.Body>
          </Card>;
        })
        return <div className='grid'>{cardItems}</div>;
    }
};

export default CardMenu;

CardMenu.propTypes = {
    cards: PropTypes.arrayOf(
        PropTypes.shape({
            image: PropTypes.string.isRequired,
            title: PropTypes.string.isRequired,
            text: PropTypes.string.isRequired,
            navigateTo: PropTypes.string.isRequired})),
    onClick: PropTypes.func.isRequired
};