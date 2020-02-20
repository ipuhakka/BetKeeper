import React, { Component } from 'react';
import PropTypes from 'prop-types';
import RBButton from 'react-bootstrap/Button';

class Button extends Component
{

    onClick()
    {
        const { props } = this;

        if (props.buttonType === 'PageAction')
        {
            /** TODO: Handlaa page action clicki. Proppina actionhandler
             * ja sille parametreinä actionUrl ja actionDataKeys.
             */
            console.log('clicked action');
        }
        else if (props.buttonType === 'ModalAction')
        {
            // TODO: Handlaa modaaliaction clicki.
            /** Handleri PageViewiin jossa modaalin renderöinti. Parametreinä fieldit ja actionUrl. */
        }
        else if (props.buttonType === 'Navigation')
        {
            // TODO: Handlaa navigointi
            console.log('clicked navigation');
        }
    }

    render()
    {
        const { props } = this;

        return <RBButton
            variant={props.style}
            onClick={this.onClick}>
                {props.text}
            </RBButton>
    }
};

Button.propTypes = {
    buttonType: PropTypes.oneOf(['ModalAction', 'PageAction', 'Navigation']).isRequired,
    text: PropTypes.string.isRequired,
    style: PropTypes.string.isRequired,
    actionUrl: PropTypes.string,
    actionDataKeys: PropTypes.arrayOf(PropTypes.string),
    navigateTo: PropTypes.string
};

export default Button;