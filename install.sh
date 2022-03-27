#!/bin/bash

set -e

podman build --tag=fd-listen-test .
cp FdListenTest.socket ~/.config/systemd/user/
cp FdListenTest.service ~/.config/systemd/user/
systemctl --user daemon-reload
systemctl --user stop FdListenTest.service
systemctl --user stop FdListenTest.socket
systemctl --user start FdListenTest.socket
