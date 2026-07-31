import { Injectable, Injector, NgZone } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HubConnection } from '@microsoft/signalr';

@Injectable()
export class ChatSignalrService extends AppComponentBase {
  constructor(
    injector: Injector,
    public _zone: NgZone,
  ) {
    super(injector);
  }

  chatHub: HubConnection;
  isChatConnected = false;

  registerChatEvents(connection: HubConnection): void {
    connection.on('getChatMessage', message => {
      eaf.event.trigger('app.chat.messageReceived', message);
    });

    connection.on('getAllFriends', friends => {
      eaf.event.trigger('eaf.chat.friendListChanged', friends);
    });

    connection.on('getFriendshipRequest', (friendData, isOwnRequest) => {
      eaf.event.trigger('app.chat.friendshipRequestReceived', friendData, isOwnRequest);
    });

    connection.on('getUserConnectNotification', (friend, isConnected) => {
      eaf.event.trigger('app.chat.userConnectionStateChanged', {
        friend: friend,
        isConnected: isConnected,
      });
    });

    connection.on('getUserStateChange', (friend, state) => {
      eaf.event.trigger('app.chat.userStateChanged', {
        friend: friend,
        state: state,
      });
    });

    connection.on('getallUnreadMessagesOfUserRead', friend => {
      eaf.event.trigger('app.chat.allUnreadMessagesOfUserRead', {
        friend: friend,
      });
    });

    connection.on('getReadStateChange', friend => {
      eaf.event.trigger('app.chat.readStateChange', {
        friend: friend,
      });
    });
  }

  sendMessage(messageData, callback): void {
    if (callback) {
      callback();
    }

    setTimeout(() => {
      eaf.event.trigger('app.chat.messageReceived', messageData);
    }, 100);
  }

  init(): void {
    this._zone.run(() => {
      this.isChatConnected = true;
      eaf.event.trigger('app.chat.connected');
    });
  }
}
